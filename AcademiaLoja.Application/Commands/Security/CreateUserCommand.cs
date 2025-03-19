using AcademiaLoja.Application.Models.Requests.Security;
using AcademiaLoja.Application.Models.Responses.Security;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Security
{
    public class CreateUserCommand :IRequest<Result<CreateUserResponse>>
    {
        public CreateUserRequest Request;
        public CreateUserCommand(CreateUserRequest request)
        {
            Request = request;
        }
    }
}
