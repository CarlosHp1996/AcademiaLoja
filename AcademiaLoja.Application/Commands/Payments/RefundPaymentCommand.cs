using AcademiaLoja.Application.Models.Responses.Payments;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Payments
{
    public class RefundPaymentCommand : IRequest<Result<RefundPaymentResponse>>
    {
        public Guid PaymentId { get; }
        public decimal Amount { get; }

        public RefundPaymentCommand(Guid paymentId, decimal amount)
        {
            PaymentId = paymentId;
            Amount = amount;
        }
    }
}
