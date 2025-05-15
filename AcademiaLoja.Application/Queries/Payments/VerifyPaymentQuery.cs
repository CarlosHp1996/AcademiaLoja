using AcademiaLoja.Application.Models.Responses.Payments;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Queries.Payments
{
    public class VerifyPaymentQuery : IRequest<Result<VerifyPaymentResponse>>
    {
        public Guid PaymentId { get; }

        public VerifyPaymentQuery(Guid paymentId)
        {
            PaymentId = paymentId;
        }
    }
}
