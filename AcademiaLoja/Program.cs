using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Services;
using AcademiaLoja.Application.Services.Interfaces;
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
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

string connectionString = Environment.GetEnvironmentVariable("ACADEMIALOJA_DB_CONNECTION") ??
                          builder.Configuration.GetConnectionString("DefaultConnection");

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddScoped<AccessManager>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICodeGeneratorService, CodeGeneratorService>();
builder.Services.AddScoped<ITemplateService, TemplateService>();
builder.Services.AddSingleton<IFileSystemService, FileSystemService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ITrackingRepository, TrackingRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IFileStorageService>(provider =>
    new FileStorageService(
        @"C:\Users\Carlos Henrique\Desktop\PROJETOSNOVOS\AcademiaLoja\ImagensBackend"
    ));
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddStripeServices(builder.Configuration); // Configura��o do Stripe

// Add Cors (chamada do frontend)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://127.0.0.1:5502") // Apenas a origem, sem o caminho
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add MediatR to the services
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

// Configurar logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Registrar servi�os usando a extens�o
builder.Services.AddCrudGenerator();

// Enabling the use of ASP.NET Identity
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

// Load JWT keys from environment variables
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("The JWT key has not been set. Set the environment variable 'JWT_KEY'.");    
}

// Authentication and JWT Configuration
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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
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

// Authorization
builder.Services.AddAuthorization();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AcademiaLoja API", Version = "v1" });

    // Configuration for Swagger to use JWT
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

    // Adds annotation support
    c.EnableAnnotations();
});

var app = builder.Build();

// Seed Roles and Admin User
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

    // Criar roles
    string[] roleNames = { "User", "Admin" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid> { Name = roleName });
        }
    }

    // Criar usu�rio Admin
    var adminUser = await userManager.FindByEmailAsync("carloshpsantos1996@gmail.com");
    if (adminUser == null)
    {
        adminUser = new ApplicationUser { UserName = "CarlosAdmin", Email = "carloshpsantos1996@gmail.com" };
        await userManager.CreateAsync(adminUser, "@Caique123");
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        @"C:\Users\Carlos Henrique\Desktop\PROJETOSNOVOS\AcademiaLoja\ImagensBackend"),
    RequestPath = "" // Sem prefixo, para que /videos/images/{fileName} funcione diretamente
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AcademiaLoja API v1");
    //c.RoutePrefix = string.Empty;
    c.RoutePrefix = "swagger";
});

app.UseStripeConfiguration();
app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
