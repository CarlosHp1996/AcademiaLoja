using AcademiaLoja.Application.Services.Interfaces;
using AcademiaLoja.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;

namespace AcademiaLoja.Application.Services
{
    public class StripePaymentService : IPaymentService
    {
        private readonly ILogger<StripePaymentService> _logger;

        public StripePaymentService(IConfiguration configuration, ILogger<StripePaymentService> logger)
        {
            // Configurar a chave secreta da Stripe
            StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY_ACADEMIAA") ??
                         configuration["Stripe:STRIPE_SECRET_KEY_ACADEMIA"];
            _logger = logger;
        }

        public async Task<Payment> CreatePaymentIntentAsync(Order order, CancellationToken cancellationToken = default)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(order.TotalAmount * 100), // Stripe trabalha com centavos
                Currency = "brl", // Moeda brasileira
                ReceiptEmail = order.User?.Email, // Email do cliente para recibo
                Metadata = new Dictionary<string, string>
                {
                    { "OrderId", order.Id.ToString() }
                },
                // Configurar métodos de pagamento automáticos
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true, // Habilita métodos automáticos
                    AllowRedirects = "never" // Desativa redirecionamentos
                }
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options, null, cancellationToken);

            // Criar um novo registro de pagamento
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                Amount = order.TotalAmount,
                PaymentMethod = "stripe",
                Status = "pending",
                Currency = "BRL",
                TransactionId = paymentIntent.Id,
                ClientSecret = paymentIntent.ClientSecret,
                CreatedAt = DateTime.UtcNow,
                PaymentDate = DateTime.UtcNow
            };

            return payment;
        }

        public async Task<bool> ConfirmPaymentAsync(string paymentIntentId, CancellationToken cancellationToken = default)
        {
            var service = new PaymentIntentService();
            var paymentIntent = await service.GetAsync(paymentIntentId, null);

            return paymentIntent?.Status == "succeeded";
        }

        public async Task<PaymentIntent> ConfirmPaymentIntentAsync(string paymentIntentId, string paymentMethodId, CancellationToken cancellationToken = default)
        {
            try
            {
                var service = new PaymentIntentService();
                var options = new PaymentIntentConfirmOptions
                {
                    PaymentMethod = paymentMethodId
                };
                var paymentIntent = await service.ConfirmAsync(paymentIntentId, options, null, cancellationToken);
                return paymentIntent;
            }
            catch (StripeException ex)
            {
                Console.WriteLine($"Erro ao confirmar PaymentIntent {paymentIntentId}: {ex.Message}");
                throw;
            }
        }

        public async Task<PaymentIntent> GetPaymentIntentAsync(string paymentIntentId, CancellationToken cancellationToken = default)
        {
            var service = new PaymentIntentService();
            return await service.GetAsync(paymentIntentId, null);
        }

        public async Task<Refund> RefundPaymentAsync(string paymentIntentId, long? amount = null, CancellationToken cancellationToken = default)
        {
            var options = new RefundCreateOptions
            {
                PaymentIntent = paymentIntentId,
                Amount = amount
            };

            var service = new RefundService();
            return await service.CreateAsync(options, null, cancellationToken);
        }

        public async Task<bool> VerifyPaymentAsync(string paymentIntentId, CancellationToken cancellationToken = default)
        {
            try
            {
                var service = new PaymentIntentService();
                var paymentIntent = await service.GetAsync(paymentIntentId, null, null, cancellationToken);

                return paymentIntent.Status == "succeeded";
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Error verifying payment");
                return false;
            }
        }

        public async Task<(bool Success, string RefundId)> RefundPaymentAsync(string paymentIntentId, decimal amount = 0, CancellationToken cancellationToken = default)
        {
            try
            {
                var options = new RefundCreateOptions
                {
                    PaymentIntent = paymentIntentId,
                    // If amount is 0, refund the entire amount
                    Amount = amount > 0 ? Convert.ToInt64(amount * 100) : null
                };

                var service = new RefundService();
                var refund = await service.CreateAsync(options, null, cancellationToken);

                return (refund.Status == "succeeded", refund.Id);
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Error refunding payment");
                return (false, null);
            }
        }
    }
}