using AcademiaLoja.Application.Models.Responses.Products;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Queries.Products
{
    public class GetProductByIdQuery : IRequest<Result<GetProductByIdResponse>>
    {
        public Guid Id { get; }

        public GetProductByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
