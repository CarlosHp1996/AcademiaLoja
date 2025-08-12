using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Services;
using AcademiaLoja.Application.Services.Interfaces;
using AcademiaLoja.Domain.Entities;
using AcademiaLoja.Domain.Entities.Security;
using AcademiaLoja.Domain.Security;
using AcademiaLoja.Infra.Data;
using AcademiaLoja.Infra.Repositories;
using AcademiaLoja.Web.Configuration;
using CrudGenerator;
using CrudGenerator.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ===== CONFIGURAÇÃO DE CONNECTION STRING =====
string connectionString = Environment.GetEnvironmentVariable("ACADEMIALOJA_DB_CONNECTION") ??
                          builder.Configuration.GetConnectionString("DefaultConnection") ??
                          throw new InvalidOperationException("Connection string not found!");

Console.WriteLine($"🔗 Connection String: {connectionString.Replace(connectionString.Split("Password=")[1].Split(";")[0], "***HIDDEN***")}");
// ===== FIM CONFIGURAÇÃO CONNECTION STRING =====

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddScoped<AccessManager>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// ===== CONFIGURAÇÃO DO DBCONTEXT COM RETRY =====
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString, // ✅ CORRIGIDO - usando a variável connectionString
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5, // Tenta 5 vezes
                maxRetryDelay: TimeSpan.FromSeconds(30), // Espera até 30 segundos entre as tentativas
                errorNumbersToAdd: null); // Usa os códigos de erro padrão do SQL Server para retentativas
        }));
// ===== FIM CONFIGURAÇÃO DBCONTEXT =====

// ===== CONFIGURAÇÃO REDIS =====
var redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION") ??
                           builder.Configuration.GetConnectionString("Redis") ??
                           "localhost:6379";

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
    options.InstanceName = "AcademiaLoja";
});

builder.Services.AddScoped<ICartService, CartService>();
// ===== FIM CONFIGURAÇÃO REDIS =====

// Registrar outros serviços
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICodeGeneratorService, CodeGeneratorService>();
builder.Services.AddScoped<ITemplateService, TemplateService>();
builder.Services.AddSingleton<IFileSystemService, FileSystemService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ITrackingRepository, TrackingRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHttpClient();
//PRODUÇÃO
// File Storage Service
builder.Services.AddScoped<IFileStorageService>(provider =>
    new FileStorageService("/app/ImagensBackend"));
//DESENVOLVIMENTO
//builder.Services.AddScoped<IFileStorageService>(provider =>
//    new FileStorageService(
//        @"C:\Users\Carlos Henrique\Desktop\PROJETOSNOVOS\AcademiaLoja\ImagensBackend"
//    ));
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUrlHelperService, UrlHelperService>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddStripeServices(builder.Configuration);

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "https://procksuplementos.com.br",
            "https://www.procksuplementos.com.br",
            //URLS PARA TESTAR FRONTEND EM AMBIENTE DEV
            "http://127.0.0.1:5502",
            "http://localhost:5502"
        )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// CrudGenerator
builder.Services.AddCrudGenerator();

// ===== IDENTITY CONFIGURATION =====
builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 1;
    options.Password.RequiredUniqueChars = 1;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();
// ===== FIM IDENTITY =====

// ===== JWT CONFIGURATION =====
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("The JWT key has not been set. Set the environment variable 'JWT_KEY'.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            var accessManager = context.HttpContext.RequestServices.GetRequiredService<AccessManager>();
            var token = context.SecurityToken as JwtSecurityToken;
            if (token != null && AccessManager.IsTokenBlacklisted(token.RawData))
            {
                context.Fail("Este token foi invalidado.");
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();
// ===== FIM JWT =====

// ===== SWAGGER CONFIGURATION =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AcademiaLoja API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    c.EnableAnnotations();
});
// ===== FIM SWAGGER =====

var app = builder.Build();

// ===== TESTE DE CONEXÃO REDIS =====
using (var scope = app.Services.CreateScope())
{
    try
    {
        var cache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();
        await cache.SetStringAsync("test-connection", "Redis conectado com sucesso!");
        var testValue = await cache.GetStringAsync("test-connection");

        if (testValue != null)
        {
            Console.WriteLine("✅ Redis conectado com sucesso!");
        }
        else
        {
            Console.WriteLine("❌ Erro na conexão com Redis");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Erro ao conectar com Redis: {ex.Message}");
        Console.WriteLine("⚠️  Verifique se o Redis está rodando na porta 6379");
    }
}
// ===== FIM TESTE REDIS =====

// ===== APLICAR MIGRATIONS E INICIALIZAR BANCO =====
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        Console.WriteLine("🔄 Verificando conexão com o banco de dados...");

        var context = services.GetRequiredService<AppDbContext>();

        // Testar conexão
        await context.Database.CanConnectAsync();
        Console.WriteLine("✅ Conexão com banco de dados estabelecida!");

        // Aplicar migrations
        Console.WriteLine("🔄 Aplicando migrations...");
        await context.Database.MigrateAsync(); // Esta linha já cria o banco se não existir e aplica as migrations
        Console.WriteLine("✅ Migrations aplicadas com sucesso!");

        Console.WriteLine("✅ Inserindo dados do produto!");
        await context.Database.MigrateAsync();
        Console.WriteLine("✅ Dados do produto inseridos!");

    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Erro crítico ao inicializar o banco de dados");
        Console.WriteLine($"❌ ERRO CRÍTICO: {ex.Message}");

        // Log da inner exception se existir
        if (ex.InnerException != null)
        {
            Console.WriteLine($"❌ ERRO INTERNO: {ex.InnerException.Message}");
        }

        Console.WriteLine("🔄 Tentando continuar mesmo com erro...");
        // Não fazer throw para permitir que o container continue rodando para debug
    }
}
// ===== FIM MIGRATIONS =====

// ===== SEED DE DADOS =====
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        Console.WriteLine("🔄 Inicializando dados do sistema...");

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        // Criar roles
        string[] roleNames = { "User", "Admin" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid> { Name = roleName });
                Console.WriteLine($"✅ Role '{roleName}' criada com sucesso!");
            }
        }

        // Criar usuário Admin
        var adminUser = await userManager.FindByEmailAsync("carloshpsantos1996@gmail.com");
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "CarlosAdmin",
                Email = "carloshpsantos1996@gmail.com",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "@Caique123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                Console.WriteLine("✅ Usuário Admin criado com sucesso!");
            }
            else
            {
                Console.WriteLine($"❌ Erro ao criar usuário Admin: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
        else
        {
            Console.WriteLine("ℹ️  Usuário Admin já existe.");
        }

        // Criar usuários Admin adicionais para teste
        for (int i = 1; i <= 5; i++)
        {
            string userName = $"admin{i}";
            string email = $"admin{i}@gmail.com";
            string password = $"@Admin{i}";

            var existingUser = await userManager.FindByEmailAsync(email);
            if (existingUser == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = userName,
                    Email = email,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(newAdmin, password);
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                    Console.WriteLine($"✅ Usuário {userName} criado com sucesso!");
                }
                else
                {
                    Console.WriteLine($"❌ Erro ao criar {userName}: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                Console.WriteLine($"ℹ️ Usuário {userName} já existe.");
            }
        }

        Console.WriteLine("✅ Inicialização de dados concluída!");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Erro ao inicializar dados do sistema");
        Console.WriteLine($"❌ ERRO AO CRIAR DADOS: {ex.Message}");
    }
}
// ===== FIM SEED =====

// ===== CONFIGURAÇÃO DE ARQUIVOS ESTÁTICOS =====
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider("/app/ImagensBackend"),
    RequestPath = "/imagens"
});
// ===== FIM ARQUIVOS ESTÁTICOS =====

// ===== PIPELINE DE MIDDLEWARE =====
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AcademiaLoja API v1");
    c.RoutePrefix = "swagger";
});

app.UseStripeConfiguration();
app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseSession();
app.UseAuthorization();
app.MapControllers();
// ===== FIM PIPELINE =====

Console.WriteLine("🚀 Aplicação iniciada com sucesso!");
Console.WriteLine($"🌐 Swagger disponível em: /swagger");

app.Run();

public static class SeedData
{
    public static async Task Initialize(AppDbContext context)
    {
        // Verifica se já existem produtos para evitar duplicação
        if (!context.Products.Any())
        {
            Console.WriteLine("🔄 Inserindo dados de produtos de exemplo...");

            context.Products.AddRange(
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Whey Protein Concentrado",
                    Description = "Whey protein de alta qualidade para ganho de massa muscular.",
                    Price = 120.00m,
                    StockQuantity = 100,
                    ImageUrl = "/imagens/whey-protein.jpg",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow                    
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Creatina Monohidratada",
                    Description = "Suplemento para aumento de força e desempenho.",
                    Price = 80.00m,
                    StockQuantity = 150,
                    ImageUrl = "/imagens/creatina.jpg",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "BCAA em Pó",
                    Description = "Aminoácidos de cadeia ramificada para recuperação muscular.",
                    Price = 75.00m,
                    StockQuantity = 80,
                    ImageUrl = "/imagens/bcaa.jpg",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );
            await context.SaveChangesAsync();
            Console.WriteLine("✅ Dados de produtos de exemplo inseridos com sucesso!");
        }
        else
        {
            Console.WriteLine("ℹ️ Produtos já existem no banco de dados. Pulando seed de produtos.");
        }
    }
}