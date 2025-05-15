namespace AcademiaLoja.Application.Models.Responses.Payments
{
    public class VerifyPaymentResponse
    {
        public Guid PaymentId { get; set; }
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string ReceiptUrl { get; set; }
    }
}
