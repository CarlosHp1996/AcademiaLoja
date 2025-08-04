using CrudGenerator.Services;

namespace CrudGenerator
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCrudGenerator(this IServiceCollection services)
        {
            // Registrar serviços
            services.AddScoped<ITemplateService, TemplateService>();

            // Registrar o serviço de sistema de arquivos
            services.AddScoped<IFileSystemService, FileSystemService>();

            // Registrar o serviço de geração de código que usa o serviço de sistema de arquivos
            services.AddScoped<ICodeGeneratorService, CodeGeneratorFileService>();

            return services;
        }
    }
}
