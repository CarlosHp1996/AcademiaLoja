using AcademiaLoja.Application.Models.Filters;
using AcademiaLoja.Application.Models.Responses.Acessory;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Queries.Accessories
{
    public class GetAccessoriesQuery : IRequest<Result<IEnumerable<AccessoryResponse>>>
    {
        public GetAccessoriessRequestFilter Filter { get; set; }

        public GetAccessoriesQuery(GetAccessoriessRequestFilter filter)
        {
            Filter = filter;
        }
    }
}
