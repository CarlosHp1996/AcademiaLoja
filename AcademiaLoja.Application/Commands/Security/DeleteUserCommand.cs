using AcademiaLoja.Application.Models.Responses.Security;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Security
{
    public class DeleteUserCommand : IRequest<Result<DeleteUserResponse>>
    {
        public Guid Id { get; }

        public DeleteUserCommand(Guid id)
        {
            Id = id;
        }
    }
}
