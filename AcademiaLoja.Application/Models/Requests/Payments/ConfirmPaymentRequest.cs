
namespace AcademiaLoja.Application.Models.Requests.Payments
{
    public class ConfirmPaymentRequest
    {
        public string PaymentIntentId { get; set; }
        public string PaymentMethodId { get; set; }
    }
}
