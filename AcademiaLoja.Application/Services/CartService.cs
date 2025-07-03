using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Requests.Orders;
using AcademiaLoja.Application.Services.Interfaces;
using AcademiaLoja.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AcademiaLoja.Application.Services
{
    public class CartService : ICartService
    {
        private readonly IDistributedCache _cache;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<CartService> _logger;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromDays(7); // Carrinho expira em 7 dias

        public CartService(
            IDistributedCache cache,
            IProductRepository productRepository,
            IOrderRepository orderRepository,
            ILogger<CartService> logger)
        {
            _cache = cache;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _logger = logger;
        }

        private string GetCartKey(string userId) => $"cart:{userId}";

        public async Task<Cart> GetCartAsync(string userId)
        {
            var cartKey = GetCartKey(userId);
            var cartJson = await _cache.GetStringAsync(cartKey);

            if (string.IsNullOrEmpty(cartJson))
            {
                var newCart = new Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _logger.LogInformation($"Created new cart for user {userId}");
                return newCart;
            }

            var cart = JsonSerializer.Deserialize<Cart>(cartJson);
            if (cart == null)
            {
                cart = new Cart { UserId = userId };
            }

            _logger.LogInformation($"Loaded cart for user {userId} - Items: {cart.Items.Count}, TotalItems: {cart.TotalItems}");
            return cart;
        }

        public async Task<Cart> AddItemAsync(string userId, CartItem newItem)
        {
            var cart = await GetCartAsync(userId);

            // Verificar se o produto existe e está ativo
            var product = await _productRepository.GetProductById(newItem.ProductId);
            if (product == null || !product.IsActive)
                throw new Exception("Produto não encontrado ou inativo");

            // Verificar estoque
            if (product.StockQuantity < newItem.Quantity)
                throw new Exception($"Estoque insuficiente. Disponível: {product.StockQuantity}");

            // Verificar se o item já existe no carrinho (mesmo produto, sabor e tamanho)
            var existingItem = cart.Items.FirstOrDefault(i =>
                i.ProductId == newItem.ProductId &&
                i.Flavor == newItem.Flavor &&
                i.Size == newItem.Size);

            if (existingItem != null)
            {
                // Verificar se a nova quantidade não excede o estoque
                var newQuantity = existingItem.Quantity + newItem.Quantity;
                if (product.StockQuantity < newQuantity)
                    throw new Exception($"Estoque insuficiente. Disponível: {product.StockQuantity}");

                existingItem.Quantity = newQuantity;
            }
            else
            {
                // Preencher dados do produto
                newItem.ProductName = product.Name;
                newItem.ProductImage = product.ImageUrl;
                newItem.UnitPrice = product.Price;
                newItem.AddedAt = DateTime.UtcNow;

                cart.Items.Add(newItem);
            }

            cart.UpdatedAt = DateTime.UtcNow;
            await SaveCartAsync(cart);
            return cart;
        }

        public async Task<Cart> UpdateItemQuantityAsync(string userId, Guid productId, int quantity)
        {
            var cart = await GetCartAsync(userId);
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (item == null)
                throw new Exception("Item não encontrado no carrinho");

            if (quantity <= 0)
            {
                cart.Items.Remove(item);
            }
            else
            {
                // Verificar estoque
                var product = await _productRepository.GetProductById(productId);
                if (product != null && product.StockQuantity < quantity)
                    throw new Exception($"Estoque insuficiente. Disponível: {product.StockQuantity}");

                item.Quantity = quantity;
            }

            cart.UpdatedAt = DateTime.UtcNow;
            await SaveCartAsync(cart);
            return cart;
        }

        public async Task<Cart> RemoveItemAsync(string userId, Guid productId)
        {
            var cart = await GetCartAsync(userId);
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (item != null)
            {
                cart.Items.Remove(item);
                cart.UpdatedAt = DateTime.UtcNow;
                await SaveCartAsync(cart);
            }

            return cart;
        }

        public async Task<bool> ClearCartAsync(string userId)
        {
            var cartKey = GetCartKey(userId);
            await _cache.RemoveAsync(cartKey);
            return true;
        }

        public async Task<int> GetCartItemCountAsync(string userId)
        {
            var cart = await GetCartAsync(userId);
            return cart.TotalItems;
        }

        public async Task<Cart> MigrateSessionCartToUserAsync(string sessionCartId, string userId)
        {
            try
            {
                _logger.LogInformation($"Migrando carrinho de sessão {sessionCartId} para usuário {userId}");

                // Buscar carrinho da sessão
                var sessionCart = await GetCartAsync(sessionCartId);

                // Se não há itens na sessão, retornar carrinho do usuário
                if (!sessionCart.Items.Any())
                {
                    return await GetCartAsync(userId);
                }

                // Buscar carrinho do usuário
                var userCart = await GetCartAsync(userId);

                // Migrar itens da sessão para o usuário
                foreach (var sessionItem in sessionCart.Items)
                {
                    // Verificar se o item já existe no carrinho do usuário
                    var existingItem = userCart.Items.FirstOrDefault(i =>
                        i.ProductId == sessionItem.ProductId &&
                        i.Flavor == sessionItem.Flavor &&
                        i.Size == sessionItem.Size);

                    if (existingItem != null)
                    {
                        // Somar quantidades
                        existingItem.Quantity += sessionItem.Quantity;
                    }
                    else
                    {
                        // Adicionar novo item
                        userCart.Items.Add(sessionItem);
                    }
                }

                // Atualizar dados do carrinho do usuário
                userCart.UserId = userId;
                userCart.UpdatedAt = DateTime.UtcNow;

                // Salvar carrinho do usuário
                await SaveCartAsync(userCart);

                // Limpar carrinho da sessão
                await ClearCartAsync(sessionCartId);

                _logger.LogInformation($"Migração concluída. {sessionCart.Items.Count} itens migrados.");

                return userCart;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao migrar carrinho de {sessionCartId} para {userId}");
                throw;
            }
        }

        public async Task<Guid> ConvertCartToOrderAsync(string userId, string shippingAddress)
        {
            var cart = await GetCartAsync(userId);

            if (!cart.Items.Any())
                throw new Exception("Carrinho está vazio");

            // Verificar se userId é um GUID válido (usuário autenticado)
            if (!Guid.TryParse(userId, out var userGuid))
                throw new Exception("Usuário deve estar autenticado para finalizar compra");

            // Converter para CreateOrderRequest
            var createOrderRequest = new CreateOrderRequest
            {
                UserId = userGuid,
                ShippingAddress = shippingAddress,
                Items = cart.Items.Select(i => new OrderItemRequest
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            };

            var orderResponse = await _orderRepository.CreateOrder(createOrderRequest, CancellationToken.None);

            // Limpar carrinho após criar pedido
            await ClearCartAsync(userId);

            _logger.LogInformation($"Pedido {orderResponse.OrderId} criado para usuário {userId}");

            return orderResponse.OrderId;
        }

        private async Task SaveCartAsync(Cart cart)
        {
            var cartKey = GetCartKey(cart.UserId);
            var cartJson = JsonSerializer.Serialize(cart);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheExpiration
            };

            await _cache.SetStringAsync(cartKey, cartJson, options);
        }
    }
}
