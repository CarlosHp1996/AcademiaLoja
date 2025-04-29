using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CrudGenerator.Models;
using CrudGenerator.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
