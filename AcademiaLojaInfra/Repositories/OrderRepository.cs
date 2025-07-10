using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Filters;
using AcademiaLoja.Application.Models.Requests.Orders;
using AcademiaLoja.Application.Models.Responses.Orders;
using AcademiaLoja.Application.Services.Interfaces;
using AcademiaLoja.Domain.Entities;
using AcademiaLoja.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace AcademiaLoja.Infra.Repositories
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        private readonly AppDbContext _context;
        private readonly IUrlHelperService _urlHelper;

        public OrderRepository(AppDbContext context, IUrlHelperService urlHelperService) : base(context)
        {
            _context = context;
            _urlHelper = urlHelperService;
        }

        public async Task<CreateOrderResponse> CreateOrder(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            // Verificar se o usu�rio existe
            var user = await _context.Users.FindAsync(new object[] { request.UserId }, cancellationToken);
            if (user == null)
                throw new Exception("User not found");

            // Verificar se h� pelo menos um item no pedido
            if (request.Items == null || !request.Items.Any())
                throw new Exception("An order must have at least one item");

            // Criar a ordem
            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Status = "Pending",
                PaymentStatus = "Pending",
                ShippingAddress = request.ShippingAddress,
                OrderDate = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TotalAmount = 0
            };

            var orderNumber = await _context.Orders
                .Where(o => o.UserId == request.UserId && o.OrderNumber > 0)
                .ToListAsync();

            if (orderNumber.Count > 0)
                order.OrderNumber = orderNumber.Max(o => o.OrderNumber) + 1;
            else
                order.OrderNumber = 1; // Primeiro pedido do usuario

            decimal totalAmount = 0;
            var orderItems = new List<OrderItem>();

            // Processar os itens do pedido
            foreach (var item in request.Items)
            {
                // Buscar o produto
                var product = await _context.Products
                    .Where(p => p.Id == item.ProductId && p.IsActive)
                    .FirstOrDefaultAsync(cancellationToken);

                if (product == null)
                    throw new Exception($"Product with ID {item.ProductId} not found or inactive");

                // Verificar disponibilidade de estoque
                if (product.StockQuantity < item.Quantity)
                    throw new Exception($"Not enough stock for product {product.Name}. Available: {product.StockQuantity}");

                // Criar o item do pedido
                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                };

                orderItems.Add(orderItem);
                totalAmount += orderItem.Quantity * orderItem.UnitPrice;

                // Atualizar estoque do produto
                product.StockQuantity -= item.Quantity;
            }

            // Atualizar o valor total do pedido
            order.TotalAmount = totalAmount;

            // Salvar a ordem e os itens no banco de dados
            using (var transaction = await _context.Database.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    await _context.Orders.AddAsync(order, cancellationToken);
                    await _context.OrderItems.AddRangeAsync(orderItems, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            }

            return new CreateOrderResponse
            {
                OrderId = order.Id,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                PaymentStatus = order.PaymentStatus,
                OrderDate = order.OrderDate,
                OrderNumber = order.OrderNumber
            };
        }

        public async Task<(IQueryable<Order> Result, int TotalCount)> Get(GetOrdersRequestFilter filter)
        {
            var query = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Payments)
                .AsQueryable();

            // Aplicar filtros
            if (filter.UserId.HasValue)
                query = query.Where(o => o.UserId == filter.UserId.Value);

            if (!string.IsNullOrEmpty(filter.Status))
                query = query.Where(o => o.Status == filter.Status);

            if (!string.IsNullOrEmpty(filter.PaymentStatus))
                query = query.Where(o => o.PaymentStatus == filter.PaymentStatus);

            if (filter.StartDate.HasValue)
                query = query.Where(o => o.OrderDate >= filter.StartDate.Value);

            if (filter.EndDate.HasValue)
                query = query.Where(o => o.OrderDate <= filter.EndDate.Value);

            if (filter.MinAmount.HasValue)
                query = query.Where(o => o.TotalAmount >= filter.MinAmount.Value);

            if (filter.MaxAmount.HasValue)
                query = query.Where(o => o.TotalAmount <= filter.MaxAmount.Value);

            // Contagem total de registros para paginaçao
            int totalCount = await query.CountAsync();

            // Ordenaçao
            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                switch (filter.SortBy.ToLower())
                {
                    case "orderdate":
                        query = filter.SortDesc
                            ? query.OrderByDescending(o => o.OrderDate)
                            : query.OrderBy(o => o.OrderDate);
                        break;
                    case "totalamount":
                        query = filter.SortDesc
                            ? query.OrderByDescending(o => o.TotalAmount)
                            : query.OrderBy(o => o.TotalAmount);
                        break;
                    case "status":
                        query = filter.SortDesc
                            ? query.OrderByDescending(o => o.Status)
                            : query.OrderBy(o => o.Status);
                        break;
                    case "paymentstatus":
                        query = filter.SortDesc
                            ? query.OrderByDescending(o => o.PaymentStatus)
                            : query.OrderBy(o => o.PaymentStatus);
                        break;
                    default:
                        query = query.OrderByDescending(o => o.OrderDate);
                        break;
                }
            }
            else
            {
                query = query.OrderByDescending(o => o.OrderDate);
            }

            // Paginaçao
            if (filter.Page.HasValue && filter.PageSize.HasValue)
            {
                int page = filter.Page.Value;
                int pageSize = filter.PageSize.Value;
                query = query.Skip((page - 1) * pageSize).Take(pageSize);
            }

            // Converter caminhos para URLs completas
            foreach (var order in query)
            {
                foreach (var item in order.OrderItems)
                {
                    if (item.Product != null)
                    {
                        item.Product.ImageUrl = _urlHelper.GenerateImageUrl(item.Product.ImageUrl);
                    }
                }
            }

            return (query, totalCount);
        }

        public async Task<Order?> GetById(Guid id, CancellationToken cancellationToken)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

            // Converter caminhos para URLs completas  
            if (order != null)
            {
                foreach (var item in order.OrderItems)
                {
                    if (item.Product != null)
                    {
                        item.Product.ImageUrl = _urlHelper.GenerateImageUrl(item.Product.ImageUrl);
                    }
                }
            }

            return order;
        }

        public async Task<UpdateOrderResponse> UpdateOrder(Guid id, UpdateOrderRequest request, CancellationToken cancellationToken)
        {
            var order = await _context.Orders.FindAsync(new object[] { id }, cancellationToken);
            if (order == null)
                throw new Exception("Order not found");

            // Atualizar os campos recebidos
            if (!string.IsNullOrEmpty(request.Status))
                order.Status = request.Status;

            if (!string.IsNullOrEmpty(request.PaymentStatus))
                order.PaymentStatus = request.PaymentStatus;

            if (!string.IsNullOrEmpty(request.ShippingAddress))
                order.ShippingAddress = request.ShippingAddress;

            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return new UpdateOrderResponse
            {
                OrderId = order.Id,
                Status = order.Status,
                PaymentStatus = order.PaymentStatus,
                UpdatedAt = order.UpdatedAt
            };
        }

        public async Task<UpdateOrderItemResponse> UpdateOrderItem(Guid orderId, Guid orderItemId, UpdateOrderItemRequest request, CancellationToken cancellationToken)
        {
            // Verificar se o pedido existe
            var order = await _context.Orders.FindAsync(new object[] { orderId }, cancellationToken);
            if (order == null)
                throw new Exception("Order not found");

            // Verificar se o item do pedido existe
            var orderItem = await _context.OrderItems
                .Include(oi => oi.Product)
                .FirstOrDefaultAsync(oi => oi.Id == orderItemId && oi.OrderId == orderId, cancellationToken);

            if (orderItem == null)
                throw new Exception("Order item not found");

            // Verificar a diferen�a de quantidade para ajustar o estoque
            int quantityDifference = request.Quantity - orderItem.Quantity;

            // Se estamos aumentando a quantidade, verificar estoque dispon�vel
            if (quantityDifference > 0)
            {
                var product = await _context.Products.FindAsync(new object[] { orderItem.ProductId }, cancellationToken);
                if (product.StockQuantity < quantityDifference)
                    throw new Exception($"Not enough stock for product {product.Name}. Available: {product.StockQuantity}");

                // Atualizar estoque
                product.StockQuantity -= quantityDifference;
            }
            else if (quantityDifference < 0)
            {
                // Estamos reduzindo a quantidade, devolver ao estoque
                var product = await _context.Products.FindAsync(new object[] { orderItem.ProductId }, cancellationToken);
                product.StockQuantity += Math.Abs(quantityDifference);
            }

            // Atualizar quantidade do item
            orderItem.Quantity = request.Quantity;

            // Recalcular o valor total do pedido
            var orderItems = await _context.OrderItems
                .Where(oi => oi.OrderId == orderId)
                .ToListAsync(cancellationToken);

            decimal totalAmount = orderItems.Sum(oi => oi.Quantity * oi.UnitPrice);
            order.TotalAmount = totalAmount;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return new UpdateOrderItemResponse
            {
                OrderItemId = orderItem.Id,
                Quantity = orderItem.Quantity,
                UnitPrice = orderItem.UnitPrice,
                TotalPrice = orderItem.Quantity * orderItem.UnitPrice
            };
        }

        public async Task<bool> DeleteOrder(Guid id, CancellationToken cancellationToken)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

            if (order == null)
                return false;

            // Restaurar estoque dos produtos
            foreach (var item in order.OrderItems)
            {
                var product = await _context.Products.FindAsync(new object[] { item.ProductId }, cancellationToken);
                if (product != null)
                {
                    product.StockQuantity += item.Quantity;
                }
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<IEnumerable<Order>> GetPendingOrders(TimeSpan pendingTime)
        {
            var cutoff = DateTime.UtcNow - pendingTime;
            return await _context.Orders
                .Where(o => o.Status == "Pending" && o.OrderDate < cutoff)
                .ToListAsync();
        }

        public async Task<bool> DeleteOrderItem(Guid orderId, Guid orderItemId, CancellationToken cancellationToken)
        {
            var order = await _context.Orders.FindAsync(new object[] { orderId }, cancellationToken);
            if (order == null)
                return false;

            var orderItem = await _context.OrderItems
                .FirstOrDefaultAsync(oi => oi.Id == orderItemId && oi.OrderId == orderId, cancellationToken);

            if (orderItem == null)
                return false;

            // Restaurar estoque do produto
            var product = await _context.Products.FindAsync(new object[] { orderItem.ProductId }, cancellationToken);
            if (product != null)
            {
                product.StockQuantity += orderItem.Quantity;
            }

            // Remover o item do pedido
            _context.OrderItems.Remove(orderItem);

            // Recalcular o valor total do pedido
            order.TotalAmount -= orderItem.Quantity * orderItem.UnitPrice;
            order.UpdatedAt = DateTime.UtcNow;

            // Verificar se h� mais itens no pedido
            var remainingItems = await _context.OrderItems
                .Where(oi => oi.OrderId == orderId && oi.Id != orderItemId)
                .CountAsync(cancellationToken);

            // Se n�o houver mais itens, excluir o pedido tamb�m
            if (remainingItems == 0)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
