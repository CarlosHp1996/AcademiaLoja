using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Orders
{
    public class DeleteOrderCommand : IRequest<Result>
    {
        public Guid OrderId { get; }

        public DeleteOrderCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
