using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Dtos;
using AcademiaLoja.Application.Models.Responses.Orders;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Queries.Orders.Handlers
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<GetOrderByIdResponse>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Result<GetOrderByIdResponse>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var result = new Result<GetOrderByIdResponse>();

            try
            {
                var order = await _orderRepository.GetById(request.OrderId, cancellationToken);

                if (order == null)
                {
                    result.WithError("Order not found");
                    return result;
                }

                var response = new GetOrderByIdResponse
                {
                    Order = new OrderDto
                    {
                        Id = order.Id,
                        UserId = order.UserId,
                        UserName = order.User?.UserName,
                        TotalAmount = order.TotalAmount,
                        Status = order.Status,
                        PaymentStatus = order.PaymentStatus,
                        ShippingAddress = order.ShippingAddress,
                        OrderDate = order.OrderDate,
                        UpdatedAt = order.UpdatedAt,
                        Items = order.OrderItems?.Select(item => new OrderItemDto
                        {
                            Id = item.Id,
                            OrderId = item.OrderId,
                            ProductId = item.ProductId,
                            ProductName = item.Product?.Name,
                            ProductImageUrl = item.Product?.ImageUrl,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice
                        }).ToList() ?? new List<OrderItemDto>(),
                        Payments = order.Payments?.Select(payment => new PaymentDto
                        {
                            Id = payment.Id,
                            OrderId = payment.OrderId,
                            PaymentMethod = payment.PaymentMethod,
                            TransactionId = payment.TransactionId,
                            Amount = payment.Amount,
                            Status = payment.Status,
                            PaymentDate = payment.PaymentDate
                        }).ToList() ?? new List<PaymentDto>()
                    }
                };

                result.Value = response;
                result.HasSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.WithError($"Error retrieving order: {ex.Message}");
                return result;
            }
        }
    }
}
