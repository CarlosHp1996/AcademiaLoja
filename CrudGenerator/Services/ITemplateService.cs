using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrudGenerator.Models;

namespace CrudGenerator.Services
{
    public interface ITemplateService
    {
        Task<string> RenderEntityTemplateAsync(EntityGenerationRequest request);
        Task<string> RenderRepositoryInterfaceTemplateAsync(EntityGenerationRequest request);
        Task<string> RenderRepositoryImplementationTemplateAsync(EntityGenerationRequest request);
        Task<string> RenderCreateCommandTemplateAsync(EntityGenerationRequest request);
        Task<string> RenderUpdateCommandTemplateAsync(EntityGenerationRequest request);
        Task<string> RenderDeleteCommandTemplateAsync(EntityGenerationRequest request);
        Task<string> RenderGetQueryTemplateAsync(EntityGenerationRequest request);
        Task<string> RenderGetByIdQueryTemplateAsync(EntityGenerationRequest request);
        Task<string> RenderCreateCommandHandlerTemplateAsync(EntityGenerationRequest request);
        Task<string> RenderUpdateCommandHandlerTemplateAsync(EntityGenerationRequest request);
        Task<string> RenderDeleteCommandHandlerTemplateAsync(EntityGenerationRequest request);
        Task<string> RenderGetQueryHandlerTemplateAsync(EntityGenerationRequest request);
        Task<string> RenderGetByIdQueryHandlerTemplateAsync(EntityGenerationRequest request);
        Task<string> RenderControllerTemplateAsync(EntityGenerationRequest request);
        Task<string> RenderDbContextConfigurationTemplateAsync(EntityGenerationRequest request);
    }
}
