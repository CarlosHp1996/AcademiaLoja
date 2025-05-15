using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Payments
{
    public class ProcessWebhookCommand : IRequest<Result>
    {
        public string JsonPayload { get; set; }
        public string StripeSignature { get; set; }

        //public ProcessWebhookCommand(string jsonPayload, string stripeSignature)
        //{
        //    JsonPayload = jsonPayload;
        //    StripeSignature = stripeSignature;
        //}
    }
}
