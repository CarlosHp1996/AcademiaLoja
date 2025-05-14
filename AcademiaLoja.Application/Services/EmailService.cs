using AcademiaLoja.Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Resend;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace AcademiaLoja.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly ResendClient _resendClient;
        private readonly string _senderEmail;
        private readonly string _senderName;

        public EmailService(IConfiguration configuration)
        {
            var apiKey = Environment.GetEnvironmentVariable("API_KEY_RESEND_ACADEMIA") ??
                         configuration["Resend:API_KEY_RESEND_ACADEMIA"];

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("A variável de ambiente 'API_KEY_RESEND_ACADEMIA' não está configurada.");

            // Usa método estático para criar o client
            _resendClient = (ResendClient)ResendClient.Create(apiKey);

            _senderEmail = Environment.GetEnvironmentVariable("SENDER_EMAIL_ACADEMIA") ??
                           configuration["Resend:SENDER_EMAIL_ACADEMIA"];

            _senderName = Environment.GetEnvironmentVariable("SENDER_NAME_ACADEMIA") ??
                          configuration["Resend:SENDER_NAME_ACADEMIA"];
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var emailMessage = new EmailMessage
            {
                From = $"{_senderName} <{_senderEmail}>",
                To = new[] { toEmail },
                Subject = subject,
                HtmlBody = body
            };

            var response = await _resendClient.EmailSendAsync(emailMessage);

            if (response.Exception != null)
            {
                throw new Exception($"Erro ao enviar email com Resend: {response.Exception.Message}");
            }
        }

        public async Task SendEmailConfirmationAsync(string email)
        {
            string subject = "Conta criada com sucesso - Power Rock Supplements";
            string body = @"
            <html>
              <body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;'>
                <table align='center' width='100%' cellpadding='0' cellspacing='0' style='max-width: 600px; background-color: #ffffff; margin: 20px auto; border-radius: 10px; box-shadow: 0 4px 10px rgba(0,0,0,0.05);'>
                  <tr>
                    <td style='background-color: #111827; padding: 30px 20px; border-top-left-radius: 10px; border-top-right-radius: 10px;'>
                      <h1 style='margin: 0; color: #ffffff; text-align: center; font-size: 24px;'>Conta criada com sucesso! 🎉</h1>
                    </td>
                  </tr>
                  <tr>
                    <td style='padding: 30px 20px; color: #333;'>
                      <p style='font-size: 16px;'>Olá,</p>
                      <p style='font-size: 16px; line-height: 1.6;'>
                        Sua conta foi criada com sucesso em nossa loja de suplementos <strong>Power Rock Supplements</strong>. Agora você pode acessar sua área de cliente, acompanhar seus pedidos, receber ofertas exclusivas e muito mais.
                      </p>
                      <p style='font-size: 16px; line-height: 1.6;'>
                        Para começar a explorar, clique no botão abaixo:
                      </p>
                      <div style='text-align: center; margin: 30px 0;'>
                        <a href='https://power-rock-ofertas.com.br' style='background-color: #10b981; color: white; padding: 14px 30px; text-decoration: none; font-size: 16px; border-radius: 6px;'>Acessar minha conta</a>
                      </div>
                      <p style='font-size: 14px; color: #666;'>Se você não criou essa conta, pode ignorar este email com segurança.</p>
                    </td>
                  </tr>
                  <tr>
                    <td style='background-color: #f3f4f6; padding: 20px; border-bottom-left-radius: 10px; border-bottom-right-radius: 10px; text-align: center;'>
                      <p style='margin: 0; font-size: 14px; color: #777;'>Dúvidas? <a href='https://colocarwhatsapp' style='color: #3b82f6; text-decoration: none;'>Fale conosco</a></p>
                      <p style='margin-top: 10px; font-size: 14px; color: #999;'>Power Rock Supplements © 2025</p>
                    </td>
                  </tr>
                </table>
              </body>
            </html>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task<bool> IsValidEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var mailAddress = new MailAddress(email);

                if (!email.Contains("@") || !email.Split('@')[1].Contains("."))
                    return false;

                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }
    }
}
