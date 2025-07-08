namespace AcademiaLoja.Application.Models.Responses.Payments
{
    public class CreatePaymentResponse
    {
        public Guid PaymentId { get; set; }
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string ClientSecret { get; set; } // For Stripe frontend integration
        public string Status { get; set; }
        public string TransactionId { get; set; }
    }
}
