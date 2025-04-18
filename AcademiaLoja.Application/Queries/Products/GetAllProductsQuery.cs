using AcademiaLoja.Application.Models.Filters;
using AcademiaLoja.Application.Models.Responses.Products;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Queries.Products
{
    public class GetAllProductsQuery : IRequest<Result<GetAllProductsResponse>>
    {
        public GetProductsRequestFilter Filter { get; }

        public GetAllProductsQuery(GetProductsRequestFilter filter)
        {
            Filter = filter;
        }
    }
}
