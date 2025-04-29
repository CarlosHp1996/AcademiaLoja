using Microsoft.Extensions.Logging;

namespace CrudGenerator.Services
{
    public interface IFileSystemService
    {
        Task<string> SaveFileAsync(string relativePath, string content);
        string GetOutputDirectory();
    }

    public class FileSystemService : IFileSystemService
    {
        //private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FileSystemService> _logger;
        private readonly string _outputDirectory;

        public FileSystemService(ILogger<FileSystemService> logger)
        {            
            _logger = logger;

            // Usar o diretório ContentRootPath como base para garantir permissões de escrita
            _outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Generated");

            // Garantir que o diretório base exista
            if (!Directory.Exists(_outputDirectory))
            {
                Directory.CreateDirectory(_outputDirectory);
                _logger.LogInformation($"Diretório de saída criado: {_outputDirectory}");
            }
        }

        public async Task<string> SaveFileAsync(string relativePath, string content)
        {
            try
            {
                var fullPath = Path.Combine(_outputDirectory, relativePath);
                var directory = Path.GetDirectoryName(fullPath);

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                    _logger.LogInformation($"Diretório criado: {directory}");
                }

                await File.WriteAllTextAsync(fullPath, content);
                _logger.LogInformation($"Arquivo salvo: {fullPath}");

                return fullPath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao salvar arquivo {relativePath}: {ex.Message}");
                throw new IOException($"Não foi possível salvar o arquivo {relativePath}: {ex.Message}", ex);
            }
        }

        public string GetOutputDirectory()
        {
            return _outputDirectory;
        }
    }
}
