using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Dtos;
using AcademiaLoja.Application.Models.Responses.Orders;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Queries.Orders.Handlers
{
    public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, Result<GetAllOrdersResponse>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetAllOrdersQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Result<GetAllOrdersResponse>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            var result = new Result<GetAllOrdersResponse>();

            try
            {
                var ordersResult = await _orderRepository.Get(request.Filter);
                var orders = ordersResult.Result.ToList();
                int totalCount = ordersResult.TotalCount;
                int pageSize = request.Filter.PageSize ?? 10;
                int pageCount = (int)Math.Ceiling(totalCount / (double)pageSize);

                var response = new GetAllOrdersResponse
                {
                    Orders = orders.Select(order => new OrderDto
                    {
                        Id = order.Id,
                        UserId = order.UserId,
                        UserName = order.User?.UserName,
                        TotalAmount = order.TotalAmount,
                        Status = order.Status,
                        PaymentStatus = order.PaymentStatus,
                        OrderNumber = order.OrderNumber,
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
                    }).ToList(),

                    Pagination = new PaginationDto
                    {
                        CurrentPage = request.Filter.Page ?? 1,
                        PageSize = pageSize,
                        TotalItems = totalCount,
                        TotalPages = pageCount
                    }
                };

                result.Value = response;
                result.HasSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.WithError($"Error retrieving orders: {ex.Message}");
                return result;
            }
        }
    }
}
