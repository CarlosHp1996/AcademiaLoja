using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Orders
{
    public class DeleteOrderItemCommand : IRequest<Result>
    {
        public Guid OrderId { get; }
        public Guid OrderItemId { get; }

        public DeleteOrderItemCommand(Guid orderId, Guid orderItemId)
        {
            OrderId = orderId;
            OrderItemId = orderItemId;
        }
    }
}
