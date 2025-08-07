using AcademiaLoja.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace AcademiaLoja.Application.Services
{
    public class UrlHelperService : IUrlHelperService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public UrlHelperService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public string GenerateImageUrl(string relativePath)
        {
            // Se o caminho está vazio ou nulo, retorna vazio
            if (string.IsNullOrEmpty(relativePath))
                return string.Empty;

            // Se já é uma URL completa (http/https), retorna como está
            if (relativePath.StartsWith("http"))
                return relativePath;

            // Normaliza o caminho removendo barras iniciais para padronizar
            relativePath = relativePath.TrimStart('/');

            // Tenta obter a URL base da requisição atual (modo dinâmico)
            var request = _httpContextAccessor.HttpContext?.Request;
            if (request != null)
            {
                var baseUrl = $"{request.Scheme}://{request.Host}";

                // ✨ AQUI ESTÁ A CORREÇÃO PRINCIPAL ✨
                // Adiciona o prefixo '/imagens/' que o Nginx está esperando
                return $"{baseUrl}/imagens/{relativePath}";
            }

            // Fallback para quando não há contexto HTTP (jobs background, etc.)
            // Usa a configuração do appsettings ou variável de ambiente
            var fallbackBaseUrl = _configuration["FileStorage:BaseUrl"] ?? "https://procksuplementos.com.br";

            // Remove barra final da URL base se existir
            fallbackBaseUrl = fallbackBaseUrl.TrimEnd('/');

            return $"{fallbackBaseUrl}/imagens/{relativePath}";
        }
    }
}