namespace AcademiaLoja.Domain.Helpers
{
    public class UserNameHelper
    {
        public static string GenerateUserName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Nome inválido");

            // Remove espaços extras e junta os nomes
            var userName = string.Concat(fullName
                .Trim()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries));

            return userName;
        }
    }
}
