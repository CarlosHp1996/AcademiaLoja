using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Responses.Payments;
using AcademiaLoja.Application.Services.Interfaces;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Payments.Handlers
{
    public class RefundPaymentCommandHandler : IRequestHandler<RefundPaymentCommand, Result<RefundPaymentResponse>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentService _paymentService;
        private readonly IOrderRepository _orderRepository;

        public RefundPaymentCommandHandler(
            IPaymentRepository paymentRepository,
            IPaymentService paymentService,
            IOrderRepository orderRepository)
        {
            _paymentRepository = paymentRepository;
            _paymentService = paymentService;
            _orderRepository = orderRepository;
        }

        public async Task<Result<RefundPaymentResponse>> Handle(RefundPaymentCommand request, CancellationToken cancellationToken)
        {
            var result = new Result<RefundPaymentResponse>();

            try
            {
                // Get the payment
                var payment = await _paymentRepository.GetByIdWithDetailsAsync(request.PaymentId, cancellationToken);

                if (payment == null)
                {
                    result.WithError("Payment not found");
                    return result;
                }

                // Check if payment can be refunded
                if (payment.Status != "succeeded")
                {
                    result.WithError("Only succeeded payments can be refunded");
                    return result;
                }

                // Determine refund amount
                decimal refundAmount = request.Amount > 0 ? request.Amount : payment.Amount;

                // Check if refund amount is valid
                if (refundAmount > payment.Amount)
                {
                    result.WithError("Refund amount cannot exceed the payment amount");
                    return result;
                }

                // Process refund with Stripe
                var (success, refundId) = await _paymentService.RefundPaymentAsync(
                    payment.TransactionId, refundAmount, cancellationToken);

                if (!success)
                {
                    result.WithError("Failed to process refund with Stripe");
                    return result;
                }

                // Update payment status
                payment.Status = refundAmount == payment.Amount ? "refunded" : "partially_refunded";
                await _paymentRepository.UpdateAsync(payment);

                // Update order status if needed
                if (refundAmount == payment.Amount)
                {
                    var order = await _orderRepository.GetById(payment.OrderId, cancellationToken);
                    if (order != null)
                    {
                        order.PaymentStatus = "Refunded";
                        if (order.Status == "Processing" || order.Status == "Pending")
                        {
                            order.Status = "Cancelled";
                        }
                        order.UpdatedAt = DateTime.UtcNow;
                        await _orderRepository.UpdateAsync(order);
                    }
                }

                // Return response
                var response = new RefundPaymentResponse
                {
                    PaymentId = payment.Id,
                    OrderId = payment.OrderId,
                    RefundAmount = refundAmount,
                    RefundId = refundId,
                    Status = payment.Status
                };

                result.Value = response;
                result.Count = 1;
                result.HasSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.WithError($"Error processing refund: {ex.Message}");
                return result;
            }
        }
    }
}
