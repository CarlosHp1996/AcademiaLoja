using AcademiaLoja.Application.Models.Responses.Security;
using AcademiaLoja.Application.Services.Interfaces;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Security.Handlers
{
    public class ForgoutPasswordCommandHandler : IRequestHandler<ForgoutPasswordCommand, Result<ForgoutPasswordResponse>>
    {
        private readonly IEmailService _emailService;

        public ForgoutPasswordCommandHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task<Result<ForgoutPasswordResponse>> Handle(ForgoutPasswordCommand request, CancellationToken cancellationToken)
        {
            var result = new Result<ForgoutPasswordResponse>();
            var email = _emailService.SendEmailForgoutPasswordAsync(request.Email);

            if (email.Exception != null)
            {
                result.WithError("User not found.");
                return result;
            }

            var response = new ForgoutPasswordResponse
            {
                Success = true,
                Message = "Email de recuperação de senha enviado com sucesso."
            };

            result.Count = 1;
            result.Value = response;
            return result;
        }
    }
}
