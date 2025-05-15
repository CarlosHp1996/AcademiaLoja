using AcademiaLoja.Domain.Entities;
using Stripe;

namespace AcademiaLoja.Application.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<Payment> CreatePaymentIntentAsync(Order order, CancellationToken cancellationToken = default);
        Task<bool> ConfirmPaymentAsync(string paymentIntentId, CancellationToken cancellationToken = default);
        Task<PaymentIntent> ConfirmPaymentIntentAsync(string paymentIntentId, string paymentMethodId, CancellationToken cancellationToken = default);
        Task<PaymentIntent> GetPaymentIntentAsync(string paymentIntentId, CancellationToken cancellationToken = default);
        Task<Refund> RefundPaymentAsync(string paymentIntentId, long? amount = null, CancellationToken cancellationToken = default);
        Task<bool> VerifyPaymentAsync(string paymentIntentId, CancellationToken cancellationToken = default);        
        Task<(bool Success, string RefundId)> RefundPaymentAsync(string paymentIntentId, decimal amount = 0, CancellationToken cancellationToken = default);
    }
}
