using AcademiaLoja.Application.Services.Interfaces;
using AcademiaLoja.Domain.Entities;
using AcademiaLoja.Domain.Enums;
using AcademiaLoja.Domain.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace AcademiaLoja.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private string GetUserId()
        {
            // Tentar pegar o ID do usuário logado
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Se não estiver logado, usar um ID de sessão persistente
            if (string.IsNullOrEmpty(userId))
            {
                const string sessionKey = "SessionId";
                var sessionId = HttpContext.Session.GetString(sessionKey);

                if (string.IsNullOrEmpty(sessionId))
                {
                    // Gerar um novo ID de sessão e armazená-lo
                    sessionId = Guid.NewGuid().ToString();
                    HttpContext.Session.SetString(sessionKey, sessionId);
                }
                userId = $"session_{sessionId}";
            }

            return userId;
        }

        [SwaggerOperation(Summary = "Get cart", Description = "Get current user cart")]
        [SwaggerResponse(200, "Success", typeof(Result<Cart>))]
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            try
            {
                var userId = GetUserId();
                //var sessionCartId = $"session_{sessionId}";
                Console.WriteLine($"Getting cart for user: {userId}");

                var cart = await _cartService.GetCartAsync(userId);
                Console.WriteLine($"Cart loaded - Items: {cart.Items.Count}, Total: {cart.TotalAmount}, TotalItems: {cart.TotalItems}");

                var result = new Result<Cart> { Value = cart, HasSuccess = true };
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting cart: {ex.Message}");
                return BadRequest(new Result<Cart> { HasSuccess = false, Errors = new[] { ex.Message } });
            }
        }

        [SwaggerOperation(Summary = "Add item to cart", Description = "Add a product to cart")]
        [SwaggerResponse(200, "Success", typeof(Result<Cart>))]
        [HttpPost("add")]
        public async Task<IActionResult> AddItem([FromBody] AddToCartRequest request)
        {
            try
            {
                var userId = GetUserId();
                //var sessionCartId = $"session_{request.SessionId}";
                var cartItem = new CartItem
                {
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    Flavor = request.Flavor,
                    Size = request.Size
                };

                var cart = await _cartService.AddItemAsync(userId, cartItem);
                return Ok(new Result<Cart> { Value = cart, HasSuccess = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new Result<Cart> { HasSuccess = false, Errors = new[] { ex.Message } });
            }
        }

        [SwaggerOperation(Summary = "Update item quantity", Description = "Update quantity of item in cart")]
        [SwaggerResponse(200, "Success", typeof(Result<Cart>))]
        [HttpPut("update/{productId}")]
        public async Task<IActionResult> UpdateQuantity(Guid productId, [FromBody] UpdateCartItemRequest request)
        {
            try
            {
                var userId = GetUserId();
                var cart = await _cartService.UpdateItemQuantityAsync(userId, productId, request.Quantity);
                return Ok(new Result<Cart> { Value = cart, HasSuccess = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new Result<Cart> { HasSuccess = false, Errors = new[] { ex.Message } });
            }
        }

        [SwaggerOperation(Summary = "Remove item from cart", Description = "Remove a product from cart")]
        [SwaggerResponse(200, "Success", typeof(Result<Cart>))]
        [HttpDelete("remove/{productId}")]
        public async Task<IActionResult> RemoveItem(Guid productId)
        {
            try
            {
                var userId = GetUserId();
                var cart = await _cartService.RemoveItemAsync(userId, productId);
                return Ok(new Result<Cart> { Value = cart, HasSuccess = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new Result<Cart> { HasSuccess = false, Errors = new[] { ex.Message } });
            }
        }

        [SwaggerOperation(Summary = "Clear cart", Description = "Remove all items from cart")]
        [SwaggerResponse(200, "Success", typeof(Result))]
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var userId = GetUserId();
                await _cartService.ClearCartAsync(userId);
                return Ok(new Result { HasSuccess = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new Result { HasSuccess = false, Errors = new[] { ex.Message } });
            }
        }

        [SwaggerOperation(Summary = "Migrate session cart to user", Description = "Migrate anonymous cart to authenticated user")]
        [SwaggerResponse(200, "Success", typeof(Result<Cart>))]
        [HttpPost("migrate")]
        [Authorize] // Requer autenticação
        public async Task<IActionResult> MigrateSessionCart([FromBody] MigrateCartRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var sessionCartId = $"session_{request.SessionId}";
                var cart = await _cartService.MigrateSessionCartToUserAsync(sessionCartId, userId);

                return Ok(new Result<Cart> { Value = cart, HasSuccess = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new Result<Cart> { HasSuccess = false, Errors = new[] { ex.Message } });
            }
        }

        [SwaggerOperation(Summary = "Get cart item count", Description = "Get total number of items in cart")]
        [SwaggerResponse(200, "Success", typeof(Result<object>))]
        [HttpGet("count")]
        public async Task<IActionResult> GetCartCount()
        {
            try
            {
                var userId = GetUserId();
                var count = await _cartService.GetCartItemCountAsync(userId);
                return Ok(new Result<object> { Value = count, HasSuccess = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new Result<object> { HasSuccess = false, Errors = new[] { ex.Message } });
            }
        }

        [SwaggerOperation(Summary = "Convert cart to order", Description = "Create order from cart items")]
        [SwaggerResponse(200, "Success", typeof(Result<object>))]
        [HttpPost("checkout")]
        [Authorize] // Requer autenticação para checkout
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var orderId = await _cartService.ConvertCartToOrderAsync(userId, request.ShippingAddress);
                return Ok(new Result<object> { Value = orderId, HasSuccess = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new Result<object> { HasSuccess = false, Errors = new[] { ex.Message } });
            }
        }
    }

    public class AddToCartRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public EnumFlavor? Flavor { get; set; }
        public string? Size { get; set; }
        //public string SessionId { get; set; }
    }

    public class UpdateCartItemRequest
    {
        public int Quantity { get; set; }
    }

    public class CheckoutRequest
    {
        public string ShippingAddress { get; set; }
    }

    public class MigrateCartRequest
    {
        public string SessionId { get; set; }
    }
}
