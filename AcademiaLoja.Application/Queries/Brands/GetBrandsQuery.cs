using AcademiaLoja.Application.Models.Filters;
using AcademiaLoja.Application.Models.Responses.Brands;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Queries.Brands
{
    public class GetBrandsQuery : IRequest<Result<IEnumerable<BrandResponse>>>
    {
        public GetBrandsRequestFilter Filter { get; }

        public GetBrandsQuery(GetBrandsRequestFilter filter)
        {
            Filter = filter;
        }
    }
}
