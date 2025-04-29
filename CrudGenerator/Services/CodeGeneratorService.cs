using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CrudGenerator.Models;

namespace CrudGenerator.Services
{
    public class CodeGeneratorService : ICodeGeneratorService
    {
        private readonly ITemplateService _templateService;

        public CodeGeneratorService(ITemplateService templateService)
        {
            _templateService = templateService;
        }

        public async Task<List<GeneratedFile>> GenerateCrudAsync(EntityGenerationRequest request)
        {
            var generatedFiles = new List<GeneratedFile>();

            // Gerar entidade
            generatedFiles.Add(await GenerateEntityAsync(request));
            
            // Gerar interface do repositório
            generatedFiles.Add(await GenerateRepositoryInterfaceAsync(request));
            
            // Gerar implementação do repositório
            generatedFiles.Add(await GenerateRepositoryImplementationAsync(request));
            
            // Gerar comandos CQRS
            generatedFiles.AddRange(await GenerateCommandsAsync(request));
            
            // Gerar queries CQRS
            generatedFiles.AddRange(await GenerateQueriesAsync(request));
            
            // Gerar handlers
            generatedFiles.AddRange(await GenerateHandlersAsync(request));
            
            // Gerar controller
            generatedFiles.Add(await GenerateControllerAsync(request));
            
            // Gerar configuração do DbContext
            generatedFiles.Add(await GenerateDbContextConfigurationAsync(request));

            return generatedFiles;
        }

        private async Task<GeneratedFile> GenerateEntityAsync(EntityGenerationRequest request)
        {
            var content = await _templateService.RenderEntityTemplateAsync(request);
            
            return new GeneratedFile
            {
                FilePath = $"Domain/Entities/{request.EntityName}.cs",
                Content = content,
                FileType = "Entity"
            };
        }

        private async Task<GeneratedFile> GenerateRepositoryInterfaceAsync(EntityGenerationRequest request)
        {
            var content = await _templateService.RenderRepositoryInterfaceTemplateAsync(request);
            
            return new GeneratedFile
            {
                FilePath = $"Application/Interfaces/I{request.EntityName}Repository.cs",
                Content = content,
                FileType = "RepositoryInterface"
            };
        }

        private async Task<GeneratedFile> GenerateRepositoryImplementationAsync(EntityGenerationRequest request)
        {
            var content = await _templateService.RenderRepositoryImplementationTemplateAsync(request);
            
            return new GeneratedFile
            {
                FilePath = $"Infra/Repositories/{request.EntityName}Repository.cs",
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
                FilePath = $"Application/Commands/{request.EntityName}s/Create{request.EntityName}Command.cs",
                Content = createCommandContent,
                FileType = "CreateCommand"
            });
            
            // Update Command
            var updateCommandContent = await _templateService.RenderUpdateCommandTemplateAsync(request);
            files.Add(new GeneratedFile
            {
                FilePath = $"Application/Commands/{request.EntityName}s/Update{request.EntityName}Command.cs",
                Content = updateCommandContent,
                FileType = "UpdateCommand"
            });
            
            // Delete Command
            var deleteCommandContent = await _templateService.RenderDeleteCommandTemplateAsync(request);
            files.Add(new GeneratedFile
            {
                FilePath = $"Application/Commands/{request.EntityName}s/Delete{request.EntityName}Command.cs",
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
                FilePath = $"Application/Queries/{request.EntityName}s/Get{request.EntityName}sQuery.cs",
                Content = getQueryContent,
                FileType = "GetQuery"
            });
            
            // GetById Query
            var getByIdQueryContent = await _templateService.RenderGetByIdQueryTemplateAsync(request);
            files.Add(new GeneratedFile
            {
                FilePath = $"Application/Queries/{request.EntityName}s/Get{request.EntityName}ByIdQuery.cs",
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
                FilePath = $"Application/Commands/{request.EntityName}s/Handlers/Create{request.EntityName}CommandHandler.cs",
                Content = createHandlerContent,
                FileType = "CreateCommandHandler"
            });
            
            // Update Command Handler
            var updateHandlerContent = await _templateService.RenderUpdateCommandHandlerTemplateAsync(request);
            files.Add(new GeneratedFile
            {
                FilePath = $"Application/Commands/{request.EntityName}s/Handlers/Update{request.EntityName}CommandHandler.cs",
                Content = updateHandlerContent,
                FileType = "UpdateCommandHandler"
            });
            
            // Delete Command Handler
            var deleteHandlerContent = await _templateService.RenderDeleteCommandHandlerTemplateAsync(request);
            files.Add(new GeneratedFile
            {
                FilePath = $"Application/Commands/{request.EntityName}s/Handlers/Delete{request.EntityName}CommandHandler.cs",
                Content = deleteHandlerContent,
                FileType = "DeleteCommandHandler"
            });
            
            // Get Query Handler
            var getHandlerContent = await _templateService.RenderGetQueryHandlerTemplateAsync(request);
            files.Add(new GeneratedFile
            {
                FilePath = $"Application/Queries/{request.EntityName}s/Handlers/Get{request.EntityName}sQueryHandler.cs",
                Content = getHandlerContent,
                FileType = "GetQueryHandler"
            });
            
            // GetById Query Handler
            var getByIdHandlerContent = await _templateService.RenderGetByIdQueryHandlerTemplateAsync(request);
            files.Add(new GeneratedFile
            {
                FilePath = $"Application/Queries/{request.EntityName}s/Handlers/Get{request.EntityName}ByIdQueryHandler.cs",
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
                FilePath = $"Web/Controllers/{request.EntityName}Controller.cs",
                Content = content,
                FileType = "Controller"
            };
        }

        private async Task<GeneratedFile> GenerateDbContextConfigurationAsync(EntityGenerationRequest request)
        {
            var content = await _templateService.RenderDbContextConfigurationTemplateAsync(request);
            
            return new GeneratedFile
            {
                FilePath = $"Infra/Data/Configuration/{request.EntityName}Configuration.cs",
                Content = content,
                FileType = "DbContextConfiguration"
            };
        }
    }
}
