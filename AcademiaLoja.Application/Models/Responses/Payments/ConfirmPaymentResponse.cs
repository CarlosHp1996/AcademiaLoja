
namespace AcademiaLoja.Application.Models.Responses.Payments
{
    public class ConfirmPaymentResponse
    {
        public string PaymentIntentId { get; set; }
        public string Status { get; set; }
        public string ClientSecret { get; set; }
    }
}
