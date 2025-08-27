using AcademiaLoja.Application.Models.Requests.Orders;
using AcademiaLoja.Application.Models.Responses.Orders;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Orders
{
    public class UpdateOrderCommand : IRequest<Result<UpdateOrderResponse>>
    {
        public Guid OrderId { get; }
        public UpdateOrderRequest Request { get; }

        public UpdateOrderCommand(Guid orderId, UpdateOrderRequest request)
        {
            OrderId = orderId;
            Request = request;
        }
    }
}
