using AcademiaLoja.Application.Models.Requests.Security;
using AcademiaLoja.Application.Models.Responses.Security;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Security
{
    public class UpdateUserCommand : IRequest<Result<UpdateUserResponse>>
    {
        public Guid Id;
        public UpdateUserRequest Request { get; set; }

        public UpdateUserCommand(Guid id, UpdateUserRequest request)
        {
            Id = id;
            Request = request;
        }
    }
}
