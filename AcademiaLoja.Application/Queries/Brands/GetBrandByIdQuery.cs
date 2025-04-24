using AcademiaLoja.Application.Models.Responses.Brands;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Queries.Brands
{
    public class GetBrandByIdQuery : IRequest<Result<BrandResponse>>
    {
        public Guid Id { get; }

        public GetBrandByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
