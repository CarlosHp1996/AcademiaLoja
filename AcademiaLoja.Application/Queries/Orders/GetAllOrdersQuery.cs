using AcademiaLoja.Application.Models.Filters;
using AcademiaLoja.Application.Models.Responses.Orders;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Queries.Orders
{
    public class GetAllOrdersQuery : IRequest<Result<GetAllOrdersResponse>>
    {
        public GetOrdersRequestFilter Filter { get; }

        public GetAllOrdersQuery(GetOrdersRequestFilter filter)
        {
            Filter = filter;
        }
    }
}
