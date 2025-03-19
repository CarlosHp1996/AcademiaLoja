using AcademiaLoja.Application.Models.Requests.Security;
using AcademiaLoja.Application.Models.Responses.Security;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Security
{
    public class CreateLoginCommand : IRequest<Result<CreateLoginResponse>>
    {
        public CreateLoginRequest Request;
        public CreateLoginCommand(CreateLoginRequest request)
        {
            Request = request;
        }
    }
}
