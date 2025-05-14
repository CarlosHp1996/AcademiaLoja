namespace AcademiaLoja.Application.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendEmailConfirmationAsync(string email);
        Task<bool> IsValidEmailAsync(string email);
    }
}
