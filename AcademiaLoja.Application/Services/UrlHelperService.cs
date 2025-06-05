using AcademiaLoja.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace AcademiaLoja.Application.Services
{
    public class UrlHelperService : IUrlHelperService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UrlHelperService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GenerateImageUrl(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return string.Empty;

            if (relativePath.StartsWith("http"))
                return relativePath;

            var request = _httpContextAccessor.HttpContext?.Request;
            if (request != null)
            {
                var baseUrl = $"{request.Scheme}://{request.Host}";
                return $"{baseUrl}/{relativePath}";
            }

            // Fallback para desenvolvimento
            return $"C:\\Users\\Carlos Henrique\\Desktop\\PROJETOSNOVOS\\AcademiaLoja\\ImagensBackend\\{relativePath}";
        }
    }
}
