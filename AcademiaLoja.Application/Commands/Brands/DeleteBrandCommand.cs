using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Brands
{
    public class DeleteBrandCommand : IRequest<Result>
    {
        public Guid Id { get; }

        public DeleteBrandCommand(Guid id)
        {
            Id = id;
        }
    }
}
