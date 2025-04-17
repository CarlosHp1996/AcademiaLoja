using AcademiaLoja.Domain.Entities.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AcademiaLoja.Domain.Security
{
    public class AccessManager
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager; // Injetar UserManager
        private static readonly ConcurrentDictionary<string, DateTime> _blacklistedTokens = new();

        public AccessManager(IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager; // Inicializar UserManager
        }

        public async Task<string> GenerateToken(ApplicationUser user) // Mudar para Task<string>
        {
            var expiresLocal = DateTime.Now.AddHours(1); // Expiração em 1 hora no horário local
            Console.WriteLine($"Token expires at: {expiresLocal} (Local)");
            Console.WriteLine($"Token expires at: {expiresLocal.ToUniversalTime()} (UTC)");

            // Obter as roles do usuário
            var userRoles = await _userManager.GetRolesAsync(user);

            // Criar as claims, incluindo as roles
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("id", user.Id.ToString()),
                new Claim("name", user.UserName)
            };

            // Adicionar cada role como uma claim
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT_KEY"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiresLocal,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task InvalidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            if (!tokenHandler.CanReadToken(token))
            {
                return; // Token inválido, nada a fazer
            }

            var jwtToken = tokenHandler.ReadJwtToken(token);
            var expires = jwtToken.ValidTo;

            // Adiciona o token à blacklist até sua data de expiração
            _blacklistedTokens.TryAdd(token, expires);
            await Task.CompletedTask;

            // Opcional: Limpeza de tokens expirados
            CleanupExpiredTokens();
        }

        public static bool IsTokenBlacklisted(string token)
        {
            if (_blacklistedTokens.TryGetValue(token, out DateTime expires))
            {
                if (DateTime.UtcNow <= expires)
                {
                    return true; // Token está na blacklist e ainda não expirou
                }
                else
                {
                    // Remove token expirado da blacklist
                    _blacklistedTokens.TryRemove(token, out _);
                }
            }
            return false;
        }

        private void CleanupExpiredTokens()
        {
            var now = DateTime.UtcNow;
            var expiredTokens = _blacklistedTokens
                .Where(kvp => kvp.Value < now)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var token in expiredTokens)
            {
                _blacklistedTokens.TryRemove(token, out _);
            }
        }
    }
}
