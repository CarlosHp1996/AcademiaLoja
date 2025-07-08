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
                var payment = await _paymentRepository.GetByIdWithDetailsAsync(request.PaymentId, cancellationToken);

                if (payment == null)
                {
                    result.WithError("Pagamento não encontrado.");
                    return result;
                }

                // Se o pagamento já foi bem-sucedido, retorne sucesso imediatamente.
                if (payment.Status == "succeeded")
                {
                    result.Value = CreateResponse(payment);
                    result.HasSuccess = true;
                    return result;
                }

                // Se o pagamento está pendente e tem um ID de transação, verifique com o Stripe.
                if (payment.Status == "pending" && !string.IsNullOrEmpty(payment.TransactionId))
                {
                    var isConfirmed = await _paymentService.VerifyPaymentAsync(payment.TransactionId, cancellationToken);

                    if (isConfirmed)
                    {
                        payment.Status = "succeeded";
                        payment.ProcessedAt = DateTime.UtcNow;
                        payment.Order.PaymentStatus = "Paid";
                        payment.Order.Status = "Processing";
                        payment.Order.UpdatedAt = DateTime.UtcNow;

                        await _paymentRepository.UpdateAsync(payment);
                    }
                    else
                    {
                        // Se não foi confirmado ainda, retorne o status atual sem erro.
                        result.WithError("O pagamento ainda está pendente de confirmação.");
                        return result;
                    }
                }
                else if (string.IsNullOrEmpty(payment.TransactionId))
                {
                    result.WithError("ID de transação não encontrado para este pagamento.");
                    return result;
                }

                result.Value = CreateResponse(payment);
                result.HasSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                // Em caso de exceção, retorne um erro claro.
                result.WithError($"Erro ao verificar o pagamento: {ex.Message}");
                return result;
            }
        }

        private VerifyPaymentResponse CreateResponse(Domain.Entities.Payment payment)
        {
            return new VerifyPaymentResponse
            {
                PaymentId = payment.Id,
                OrderId = payment.OrderId,
                Amount = payment.Amount,
                Status = payment.Status,
                PaymentMethod = payment.PaymentMethod,
                ProcessedAt = payment.ProcessedAt,
                ReceiptUrl = payment.ReceiptUrl
            };
        }
    }
}
