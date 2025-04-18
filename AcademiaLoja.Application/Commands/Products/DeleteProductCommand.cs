using AcademiaLoja.Application.Models.Responses.Products;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Products
{
    public class DeleteProductCommand : IRequest<Result>
    {
        public Guid Id { get; }

        public DeleteProductCommand(Guid id)
        {
            Id = id;
        }
    }
}
