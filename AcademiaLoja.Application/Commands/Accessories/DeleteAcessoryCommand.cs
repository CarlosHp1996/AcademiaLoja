using AcademiaLoja.Application.Models.Responses.Acessory;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Acessory
{
    public class DeleteAcessoryCommand : IRequest<Result<AccessoryResponse>>
    {
        public Guid Id { get; set; }
        public DeleteAcessoryCommand(Guid id)
        {
            Id = id;
        }
    }
}
