using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Responses.Payments;
using AcademiaLoja.Application.Services.Interfaces;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Queries.Payments.Handlers
{
    public class VerifyPaymentQueryHandler : IRequestHandler<VerifyPaymentQuery, Result<VerifyPaymentResponse>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentService _paymentService;

        public VerifyPaymentQueryHandler(
            IPaymentRepository paymentRepository,
            IPaymentService paymentService)
        {
            _paymentRepository = paymentRepository;
            _paymentService = paymentService;
        }

        public async Task<Result<VerifyPaymentResponse>> Handle(VerifyPaymentQuery request, CancellationToken cancellationToken)
        {
            var result = new Result<VerifyPaymentResponse>();

            try
            {
                // Get payment from database
                var payment = await _paymentRepository.GetByIdWithDetailsAsync(request.PaymentId, cancellationToken);

                if (payment == null)
                {
                    result.WithError("Payment not found");
                    return result;
                }

                // If payment is still pending and has a transaction ID, check status with Stripe
                if (payment.Status == "pending" && !string.IsNullOrEmpty(payment.TransactionId))
                {
                    var isConfirmed = await _paymentService.VerifyPaymentAsync(payment.TransactionId, cancellationToken);

                    if (isConfirmed && payment.Status != "succeeded")
                    {
                        // Update payment status in database
                        payment.Status = "succeeded";
                        payment.ProcessedAt = DateTime.UtcNow;
                        await _paymentRepository.UpdateAsync(payment);

                        // Update order status if needed
                        if (payment.Order.PaymentStatus != "Paid")
                        {
                            payment.Order.PaymentStatus = "Paid";
                            if (payment.Order.Status == "Pending")
                            {
                                payment.Order.Status = "Processing";
                            }
                            payment.Order.UpdatedAt = DateTime.UtcNow;
                            // Order will be updated through EF tracking
                        }
                    }
                }

                // Return response with current payment status
                var response = new VerifyPaymentResponse
                {
                    PaymentId = payment.Id,
                    OrderId = payment.OrderId,
                    Amount = payment.Amount,
                    Status = payment.Status,
                    PaymentMethod = payment.PaymentMethod,
                    ProcessedAt = payment.ProcessedAt,
                    ReceiptUrl = payment.ReceiptUrl
                };

                result.Value = response;
                result.Count = 1;
                result.HasSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.WithError($"Error verifying payment: {ex.Message}");
                return result;
            }
        }
    }
}
