using AcademiaLoja.Application.Models.Filters;
using AcademiaLoja.Application.Models.Requests.Orders;
using AcademiaLoja.Application.Models.Responses.Orders;
using AcademiaLoja.Domain.Entities;

namespace AcademiaLoja.Application.Interfaces
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        Task<CreateOrderResponse> CreateOrder(CreateOrderRequest request, CancellationToken cancellationToken);

        // Read
        Task<(IQueryable<Order> Result, int TotalCount)> Get(GetOrdersRequestFilter filter);
        Task<Order> GetById(Guid id, CancellationToken cancellationToken);
        // Update
        Task<UpdateOrderResponse> UpdateOrder(Guid id, UpdateOrderRequest request, CancellationToken cancellationToken);
        Task<UpdateOrderItemResponse> UpdateOrderItem(Guid orderId, Guid orderItemId, UpdateOrderItemRequest request, CancellationToken cancellationToken);
        // Delete
        Task<bool> DeleteOrder(Guid id, CancellationToken cancellationToken);
        Task<bool> DeleteOrderItem(Guid orderId, Guid orderItemId, CancellationToken cancellationToken);
    }
}
