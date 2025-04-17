using AcademiaLoja.Application.Models.Requests.Security;
using AcademiaLoja.Application.Models.Responses.Security;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Security
{
    public class UpdateUserCommand : IRequest<Result<UpdateUserResponse>>
    {
        public UpdateUserRequest Request { get; }

        public UpdateUserCommand(UpdateUserRequest request)
        {
            Request = request;
        }
    }
}
