using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Responses.Orders;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Orders.Handlers
{
    public class UpdateOrderItemCommandHandler : IRequestHandler<UpdateOrderItemCommand, Result<UpdateOrderItemResponse>>
    {
        private readonly IOrderRepository _orderRepository;

        public UpdateOrderItemCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Result<UpdateOrderItemResponse>> Handle(UpdateOrderItemCommand request, CancellationToken cancellationToken)
        {
            var result = new Result<UpdateOrderItemResponse>();

            try
            {
                var response = await _orderRepository.UpdateOrderItem(request.OrderId, request.OrderItemId, request.Request, cancellationToken);

                result.Value = response;
                result.Count = 1;
                result.HasSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.WithError($"Error updating order item: {ex.Message}");
                return result;
            }
        }
    }
}
