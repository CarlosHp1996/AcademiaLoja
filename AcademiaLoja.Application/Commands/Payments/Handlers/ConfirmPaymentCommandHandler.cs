
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using AcademiaLoja.Application.Services.Interfaces;
using AcademiaLoja.Domain.Helpers;
using AcademiaLoja.Application.Models.Responses.Payments;

namespace AcademiaLoja.Application.Commands.Payments.Handlers
{
    public class ConfirmPaymentCommandHandler : IRequestHandler<ConfirmPaymentCommand, Result<ConfirmPaymentResponse>>
    {
        private readonly IPaymentService _paymentService;

        public ConfirmPaymentCommandHandler(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task<Result<ConfirmPaymentResponse>> Handle(ConfirmPaymentCommand request, CancellationToken cancellationToken)
        {
            var result = new Result<ConfirmPaymentResponse>();
            var paymentIntent = await _paymentService.ConfirmPaymentIntentAsync(request.PaymentIntentId, request.PaymentMethodId, cancellationToken);

            if (paymentIntent == null)
            {
                result.WithError("Falha ao confirmar o pagamento.");
                return result;
            }

            var response = new ConfirmPaymentResponse
            {
                PaymentIntentId = paymentIntent.Id,
                Status = paymentIntent.Status,
                ClientSecret = paymentIntent.ClientSecret
            };

            result.Value = response;
            result.Count = 1;
            result.HasSuccess = true;
            return result;
        }
    }
}
