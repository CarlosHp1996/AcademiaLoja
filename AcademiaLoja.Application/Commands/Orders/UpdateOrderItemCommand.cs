using AcademiaLoja.Application.Models.Requests.Orders;
using AcademiaLoja.Application.Models.Responses.Orders;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Orders
{
    public class UpdateOrderItemCommand : IRequest<Result<UpdateOrderItemResponse>>
    {
        public Guid OrderId { get; }
        public Guid OrderItemId { get; }
        public UpdateOrderItemRequest Request { get; }

        public UpdateOrderItemCommand(Guid orderId, Guid orderItemId, UpdateOrderItemRequest request)
        {
            OrderId = orderId;
            OrderItemId = orderItemId;
            Request = request;
        }
    }
}
