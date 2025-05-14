using AcademiaLoja.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace AcademiaLoja.Application.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _baseDirectory;

        public FileStorageService(string baseDirectory)
        {
            _baseDirectory = string.IsNullOrWhiteSpace(baseDirectory)
                ? throw new ArgumentNullException(nameof(baseDirectory))
                : baseDirectory;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string containerName, string fileName)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Nenhum arquivo fornecido ou arquivo vazio.", nameof(file));
            if (string.IsNullOrWhiteSpace(containerName))
                throw new ArgumentException("O nome do contêiner é obrigatório.", nameof(containerName));
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("O nome do arquivo é obrigatório.", nameof(fileName));

            string sanitizedFileName = Path.GetFileName(fileName.Replace(" ", "_").Replace("\\", "").Replace("/", ""));
            string folderPath = Path.Combine(_baseDirectory, containerName);
            Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, sanitizedFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Retorna apenas o caminho relativo
            return $"{containerName}/{sanitizedFileName}";
        }

        public Task<bool> DeleteFileAsync(string containerName, string fileName)
        {
            if (string.IsNullOrWhiteSpace(containerName) || string.IsNullOrWhiteSpace(fileName))
                return Task.FromResult(false);

            string filePath = Path.Combine(_baseDirectory, containerName, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}
