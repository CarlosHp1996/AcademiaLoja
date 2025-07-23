using AcademiaLoja.Application.Models.Responses.Security;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Security
{
    public class ForgoutPasswordCommand : IRequest<Result<ForgoutPasswordResponse>>
    {
        public string Email { get; set; }

        public ForgoutPasswordCommand(string email)
        {
            Email = email;
        }
    }
}
