using AcademiaLoja.Application.Models.Responses.Payments;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Payments
{
    public class CreatePaymentCommand : IRequest<Result<CreatePaymentResponse>>
    {
        public Guid OrderId { get; }

        public CreatePaymentCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
