using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CrudGenerator.Models;
using Microsoft.Extensions.Logging;

namespace CrudGenerator.Services
{
    public class CodeGeneratorFileService : ICodeGeneratorService
    {
        private readonly ITemplateService _templateService;
        private readonly IFileSystemService _fileSystemService;
        private readonly ILogger<CodeGeneratorFileService> _logger;

        public CodeGeneratorFileService(
            ITemplateService templateService, 
            IFileSystemService fileSystemService,
            ILogger<CodeGeneratorFileService> logger)
        {
            _templateService = templateService;
            _fileSystemService = fileSystemService;
            _logger = logger;
        }

        public async Task<List<GeneratedFile>> GenerateCrudAsync(EntityGenerationRequest request)
        {
            _logger.LogInformation($"Iniciando geração de CRUD para entidade {request.EntityName}");
            var generatedFiles = new List<GeneratedFile>();

            try
            {
                // Gerar entidade
                var entityFile = await GenerateEntityAsync(request);
                generatedFiles.Add(entityFile);
                await SaveFileAsync(entityFile);
                
                // Gerar interface do repositório
                var repoInterfaceFile = await GenerateRepositoryInterfaceAsync(request);
                generatedFiles.Add(repoInterfaceFile);
                await SaveFileAsync(repoInterfaceFile);
                
                // Gerar implementação do repositório
                var repoImplFile = await GenerateRepositoryImplementationAsync(request);
                generatedFiles.Add(repoImplFile);
                await SaveFileAsync(repoImplFile);
                
                // Gerar comandos CQRS
                var commandFiles = await GenerateCommandsAsync(request);
                generatedFiles.AddRange(commandFiles);
                foreach (var file in commandFiles)
                {
                    await SaveFileAsync(file);
                }
                
                // Gerar queries CQRS
                var queryFiles = await GenerateQueriesAsync(request);
                generatedFiles.AddRange(queryFiles);
                foreach (var file in queryFiles)
                {
                    await SaveFileAsync(file);
                }
                
                // Gerar handlers
                var handlerFiles = await GenerateHandlersAsync(request);
                generatedFiles.AddRange(handlerFiles);
                foreach (var file in handlerFiles)
                {
                    await SaveFileAsync(file);
                }
                
                // Gerar controller
                var controllerFile = await GenerateControllerAsync(request);
                generatedFiles.Add(controllerFile);
                await SaveFileAsync(controllerFile);
                
                // Gerar configuração do DbContext
                var dbContextFile = await GenerateDbContextConfigurationAsync(request);
                generatedFiles.Add(dbContextFile);
                await SaveFileAsync(dbContextFile);

                // Gerar modelos de request e response
                var modelFiles = await GenerateModelFilesAsync(request);
                generatedFiles.AddRange(modelFiles);
                foreach (var file in modelFiles)
                {
                    await SaveFileAsync(file);
                }

                // Gerar instruções para atualização do AppDbContext
                var dbContextUpdateFile = await GenerateDbContextUpdateInstructionsAsync(request);
                generatedFiles.Add(dbContextUpdateFile);
                await SaveFileAsync(dbContextUpdateFile);

                _logger.LogInformation($"Geração de CRUD para entidade {request.EntityName} concluída com sucesso. {generatedFiles.Count} arquivos gerados.");
                return generatedFiles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao gerar CRUD para entidade {request.EntityName}: {ex.Message}");
                throw;
            }
        }

        private async Task SaveFileAsync(GeneratedFile file)
        {
            try
            {
                var savedPath = await _fileSystemService.SaveFileAsync(file.FilePath, file.Content);
                _logger.LogInformation($"Arquivo salvo: {savedPath}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao salvar arquivo {file.FilePath}: {ex.Message}");
                throw;
            }
        }

        private async Task<GeneratedFile> GenerateEntityAsync(EntityGenerationRequest request)
        {
            var content = await _templateService.RenderEntityTemplateAsync(request);
            
            return new GeneratedFile
            {
                FilePath = $"{request.BaseNamespace}/Domain/Entities/{request.EntityName}.cs",
                Content = content,
                FileType = "Entity"
            };
        }

        private async Task<GeneratedFile> GenerateRepositoryInterfaceAsync(EntityGenerationRequest request)
        {
            var content = await _templateService.RenderRepositoryInterfaceTemplateAsync(request);
            
            return new GeneratedFile
            {
                FilePath = $"{request.BaseNamespace}/Application/Interfaces/I{request.EntityName}Repository.cs",
                Content = content,
                FileType = "RepositoryInterface"
            };
        }

        private async Task<GeneratedFile> GenerateRepositoryImplementationAsync(EntityGenerationRequest request)
        {
            var content = await _templateService.RenderRepositoryImplementationTemplateAsync(request);
            
            return new GeneratedFile
            {
                FilePath = $"{request.BaseNamespace}/Infra/Repositories/{request.EntityName}Repository.cs",
                Content = content,
                FileType = "RepositoryImplementation"
            };
        }

        private async Task<List<GeneratedFile>> GenerateCommandsAsync(EntityGenerationRequest request)
        {
            var files = new List<GeneratedFile>();
            
            // Create Command
            var createCommandContent = await _templateService.RenderCreateCommandTemplateAsync(request);
            files.Add(new GeneratedFile
            {
                FilePath = $"{request.BaseNamespace}/Application/Commands/{request.EntityName}s/Create{request.EntityName}Command.cs",
                Content = createCommandContent,
                FileType = "CreateCommand"
            });
            
            // Update Command
            var updateCommandContent = await _templateService.RenderUpdateCommandTemplateAsync(request);
            files.Add(new GeneratedFile
            {
                FilePath = $"{request.BaseNamespace}/Application/Commands/{request.EntityName}s/Update{request.EntityName}Command.cs",
                Content = updateCommandContent,
                FileType = "UpdateCommand"
            });
            
            // Delete Command
            var deleteCommandContent = await _templateService.RenderDeleteCommandTemplateAsync(request);
            files.Add(new GeneratedFile
            {
                FilePath = $"{request.BaseNamespace}/Application/Commands/{request.EntityName}s/Delete{request.EntityName}Command.cs",
                Content = deleteCommandContent,
                FileType = "DeleteCommand"
            });
            
            return files;
        }

        private async Task<List<GeneratedFile>> GenerateQueriesAsync(EntityGenerationRequest request)
        {
            var files = new List<GeneratedFile>();
            
            // Get Query
            var getQueryContent = await _templateService.RenderGetQueryTemplateAsync(request);
            files.Add(new GeneratedFile
            {
                FilePath = $"{request.BaseNamespace}/Application/Queries/{request.EntityName}s/Get{request.EntityName}sQuery.cs",
                Content = getQueryContent,
                FileType = "GetQuery"
            });
            
            // GetById Query
            var getByIdQueryContent = await _templateService.RenderGetByIdQueryTemplateAsync(request);
            files.Add(new GeneratedFile
            {
                FilePath = $"{request.BaseNamespace}/Application/Queries/{request.EntityName}s/Get{request.EntityName}ByIdQuery.cs",
                Content = getByIdQueryContent,
                FileType = "GetByIdQuery"
            });
            
            return files;
        }

        private async Task<List<GeneratedFile>> GenerateHandlersAsync(EntityGenerationRequest request)
        {
            var files = new List<GeneratedFile>();
            
            // Create Command Handler
            var createHandlerContent = await _templateService.RenderCreateCommandHandlerTemplateAsync(request);
            files.Add(new GeneratedFile
            {
                FilePath = $"{request.BaseNamespace}/Application/Commands/{request.EntityName}s/Handlers/Create{request.EntityName}CommandHandler.cs",
                Content = createHandlerContent,
                FileType = "CreateCommandHandler"
            });
            
            // Update Command Handler
            var updateHandlerContent = await _templateService.RenderUpdateCommandHandlerTemplateAsync(request);
            files.Add(new GeneratedFile
            {
                FilePath = $"{request.BaseNamespace}/Application/Commands/{request.EntityName}s/Handlers/Update{request.EntityName}CommandHandler.cs",
                Content = updateHandlerContent,
                FileType = "UpdateCommandHandler"
            });
            
            // Delete Command Handler
            var deleteHandlerContent = await _templateService.RenderDeleteCommandHandlerTemplateAsync(request);
            files.Add(new GeneratedFile
            {
                FilePath = $"{request.BaseNamespace}/Application/Commands/{request.EntityName}s/Handlers/Delete{request.EntityName}CommandHandler.cs",
                Content = deleteHandlerContent,
                FileType = "DeleteCommandHandler"
            });
            
            // Get Query Handler
            var getHandlerContent = await _templateService.RenderGetQueryHandlerTemplateAsync(request);
            files.Add(new GeneratedFile
            {
                FilePath = $"{request.BaseNamespace}/Application/Queries/{request.EntityName}s/Handlers/Get{request.EntityName}sQueryHandler.cs",
                Content = getHandlerContent,
                FileType = "GetQueryHandler"
            });
            
            // GetById Query Handler
            var getByIdHandlerContent = await _templateService.RenderGetByIdQueryHandlerTemplateAsync(request);
            files.Add(new GeneratedFile
            {
                FilePath = $"{request.BaseNamespace}/Application/Queries/{request.EntityName}s/Handlers/Get{request.EntityName}ByIdQueryHandler.cs",
                Content = getByIdHandlerContent,
                FileType = "GetByIdQueryHandler"
            });
            
            return files;
        }

        private async Task<GeneratedFile> GenerateControllerAsync(EntityGenerationRequest request)
        {
            var content = await _templateService.RenderControllerTemplateAsync(request);
            
            return new GeneratedFile
            {
                FilePath = $"{request.BaseNamespace}/Web/Controllers/{request.EntityName}Controller.cs",
                Content = content,
                FileType = "Controller"
            };
        }

        private async Task<GeneratedFile> GenerateDbContextConfigurationAsync(EntityGenerationRequest request)
        {
            var content = await _templateService.RenderDbContextConfigurationTemplateAsync(request);
            
            return new GeneratedFile
            {
                FilePath = $"{request.BaseNamespace}/Infra/Data/Configuration/{request.EntityName}Configuration.cs",
                Content = content,
                FileType = "DbContextConfiguration"
            };
        }

        private async Task<List<GeneratedFile>> GenerateModelFilesAsync(EntityGenerationRequest request)
        {
            var files = new List<GeneratedFile>();
            
            // Create Request
            var sb = new StringBuilder();
            sb.AppendLine($"using System;");
            sb.AppendLine($"using System.ComponentModel.DataAnnotations;");
            sb.AppendLine();
            sb.AppendLine($"namespace {request.BaseNamespace}.Application.Models.Requests.{request.EntityName}s");
            sb.AppendLine("{");
            sb.AppendLine($"    public class Create{request.EntityName}Request");
            sb.AppendLine("    {");
            
            foreach (var property in request.Properties)
            {
                if (!string.IsNullOrEmpty(property.Description))
                {
                    sb.AppendLine($"        /// <summary>");
                    sb.AppendLine($"        /// {property.Description}");
                    sb.AppendLine($"        /// </summary>");
                }
                
                if (property.IsRequired)
                {
                    sb.AppendLine("        [Required]");
                }
                
                if (property.Type.ToLower() == "string" && property.MaxLength.HasValue)
                {
                    sb.AppendLine($"        [MaxLength({property.MaxLength.Value})]");
                }
                
                sb.AppendLine($"        public {property.Type} {property.Name} {{ get; set; }}");
                sb.AppendLine();
            }
            
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            files.Add(new GeneratedFile
            {
                FilePath = $"{request.BaseNamespace}/Application/Models/Requests/{request.EntityName}s/Create{request.EntityName}Request.cs",
                Content = sb.ToString(),
                FileType = "CreateRequest"
            });
            
            // Update Request
            sb = new StringBuilder();
            sb.AppendLine($"using System;");
            sb.AppendLine($"using System.ComponentModel.DataAnnotations;");
            sb.AppendLine();
            sb.AppendLine($"namespace {request.BaseNamespace}.Application.Models.Requests.{request.EntityName}s");
            sb.AppendLine("{");
            sb.AppendLine($"    public class Update{request.EntityName}Request");
            sb.AppendLine("    {");
            
            foreach (var property in request.Properties)
            {
                if (!string.IsNullOrEmpty(property.Description))
                {
                    sb.AppendLine($"        /// <summary>");
                    sb.AppendLine($"        /// {property.Description}");
                    sb.AppendLine($"        /// </summary>");
                }
                
                if (property.IsRequired)
                {
                    sb.AppendLine("        [Required]");
                }
                
                if (property.Type.ToLower() == "string" && property.MaxLength.HasValue)
                {
                    sb.AppendLine($"        [MaxLength({property.MaxLength.Value})]");
                }
                
                sb.AppendLine($"        public {property.Type} {property.Name} {{ get; set; }}");
                sb.AppendLine();
            }
            
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            files.Add(new GeneratedFile
            {
                FilePath = $"{request.BaseNamespace}/Application/Models/Requests/{request.EntityName}s/Update{request.EntityName}Request.cs",
                Content = sb.ToString(),
                FileType = "UpdateRequest"
            });
            
            // Response
            sb = new StringBuilder();
            sb.AppendLine($"using System;");
            sb.AppendLine();
            sb.AppendLine($"namespace {request.BaseNamespace}.Application.Models.Responses.{request.EntityName}s");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {request.EntityName}Response");
            sb.AppendLine("    {");
            sb.AppendLine("        public Guid Id { get; set; }");
            
            foreach (var property in request.Properties)
            {
                if (!string.IsNullOrEmpty(property.Description))
                {
                    sb.AppendLine($"        /// <summary>");
                    sb.AppendLine($"        /// {property.Description}");
                    sb.AppendLine($"        /// </summary>");
                }
                
                sb.AppendLine($"        public {property.Type} {property.Name} {{ get; set; }}");
            }
            
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            files.Add(new GeneratedFile
            {
                FilePath = $"{request.BaseNamespace}/Application/Models/Responses/{request.EntityName}s/{request.EntityName}Response.cs",
                Content = sb.ToString(),
                FileType = "Response"
            });
            
            // Filter
            sb = new StringBuilder();
            sb.AppendLine($"using System;");
            sb.AppendLine();
            sb.AppendLine($"namespace {request.BaseNamespace}.Application.Models.Filters");
            sb.AppendLine("{");
            sb.AppendLine($"    public class Get{request.EntityName}sRequestFilter : BaseRequestFilter");
            sb.AppendLine("    {");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            files.Add(new GeneratedFile
            {
                FilePath = $"{request.BaseNamespace}/Application/Models/Filters/Get{request.EntityName}sRequestFilter.cs",
                Content = sb.ToString(),
                FileType = "Filter"
            });
            
            return files;
        }

        private async Task<GeneratedFile> GenerateDbContextUpdateInstructionsAsync(EntityGenerationRequest request)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("# Instruções para atualizar o AppDbContext");
            sb.AppendLine();
            sb.AppendLine("## 1. Adicione a propriedade DbSet no AppDbContext.cs");
            sb.AppendLine();
            sb.AppendLine("```csharp");
            sb.AppendLine($"public DbSet<{request.EntityName}> {request.EntityName}s {{ get; set; }}");
            sb.AppendLine("```");
            sb.AppendLine();
            sb.AppendLine("## 2. Adicione a configuração no método OnModelCreating");
            sb.AppendLine();
            sb.AppendLine("```csharp");
            sb.AppendLine($"// Configuração de {request.EntityName}");
            sb.AppendLine($"modelBuilder.Entity<{request.EntityName}>(entity =>");
            sb.AppendLine("{");
            sb.AppendLine($"    entity.ToTable(\"{request.EntityName}s\");");
            sb.AppendLine("    entity.HasKey(c => c.Id);");
            sb.AppendLine();
            
            foreach (var property in request.Properties)
            {
                if (property.Type.ToLower() == "string")
                {
                    sb.AppendLine($"    entity.Property(c => c.{property.Name})");
                    
                    if (property.IsRequired)
                    {
                        sb.AppendLine("        .IsRequired()");
                    }
                    
                    if (property.MaxLength.HasValue)
                    {
                        sb.AppendLine($"        .HasMaxLength({property.MaxLength.Value})");
                    }
                    
                    sb.AppendLine("        ;");
                    sb.AppendLine();
                }
            }
            
            sb.AppendLine("    // Relacionamentos");
            sb.AppendLine($"    entity.HasMany(c => c.Product{request.EntityName}s)");
            sb.AppendLine($"        .WithOne(pc => pc.{request.EntityName})");
            sb.AppendLine($"        .HasForeignKey(pc => pc.{request.EntityName}Id);");
            sb.AppendLine("});");
            sb.AppendLine("```");
            sb.AppendLine();
            //sb.AppendLine("## 3. Crie a classe de relacionamento Product" + request.EntityName);
            //sb.AppendLine();
            //sb.AppendLine("```csharp");
            //sb.AppendLine($"public class Product{request.EntityName}");
            //sb.AppendLine("{");
            //sb.AppendLine("    public Guid ProductId { get; set; }");
            //sb.AppendLine($"    public Guid {request.EntityName}Id {{ get; set; }}");
            //sb.AppendLine();
            //sb.AppendLine("    // Navegação");
            //sb.AppendLine("    public virtual Product Product { get; set; }");
            //sb.AppendLine($"    public virtual {request.EntityName} {request.EntityName} {{ get; set; }}");
            //sb.AppendLine("}");
            //sb.AppendLine("```");
            //sb.AppendLine();
            //sb.AppendLine("## 4. Adicione a propriedade DbSet para o relacionamento");
            //sb.AppendLine();
            //sb.AppendLine("```csharp");
            //sb.AppendLine($"public DbSet<Product{request.EntityName}> Product{request.EntityName}s {{ get; set; }}");
            //sb.AppendLine("```");
            //sb.AppendLine();
            //sb.AppendLine("## 5. Adicione a configuração do relacionamento no método OnModelCreating");
            //sb.AppendLine();
            //sb.AppendLine("```csharp");
            //sb.AppendLine($"// Configuração de Product{request.EntityName}");
            //sb.AppendLine($"modelBuilder.Entity<Product{request.EntityName}>(entity =>");
            //sb.AppendLine("{");
            //sb.AppendLine($"    entity.ToTable(\"Product{request.EntityName}s\");");
            //sb.AppendLine($"    entity.HasKey(pc => new {{ pc.ProductId, pc.{request.EntityName}Id }});");
            //sb.AppendLine("});");
            //sb.AppendLine("```");
            
            return new GeneratedFile
            {
                FilePath = $"{request.BaseNamespace}/ReadMe/{request.EntityName}/DbContextUpdateInstructions.md",
                Content = sb.ToString(),
                FileType = "Instructions"
            };
        }
    }
}
