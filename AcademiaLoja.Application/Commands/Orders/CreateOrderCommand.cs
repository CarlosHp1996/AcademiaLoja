using AcademiaLoja.Application.Models.Requests.Orders;
using AcademiaLoja.Application.Models.Responses.Orders;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Orders
{
    public class CreateOrderCommand : IRequest<Result<CreateOrderResponse>>
    {
        public CreateOrderRequest Request;
        public CreateOrderCommand(CreateOrderRequest request)
        {
            Request = request;
        }
    }
}
