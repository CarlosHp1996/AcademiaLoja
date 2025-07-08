
using MediatR;
using AcademiaLoja.Domain.Helpers;
using AcademiaLoja.Application.Models.Responses.Payments;

namespace AcademiaLoja.Application.Commands.Payments
{
    public class ConfirmPaymentCommand : IRequest<Result<ConfirmPaymentResponse>>
    {
        public string PaymentIntentId { get; }
        public string PaymentMethodId { get; }

        public ConfirmPaymentCommand(string paymentIntentId, string paymentMethodId)
        {
            PaymentIntentId = paymentIntentId;
            PaymentMethodId = paymentMethodId;
        }
    }
}
