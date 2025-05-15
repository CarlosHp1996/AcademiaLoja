namespace AcademiaLoja.Application.Models.Responses.Payments
{
    public class RefundPaymentResponse
    {
        public Guid PaymentId { get; set; }
        public Guid OrderId { get; set; }
        public decimal RefundAmount { get; set; }
        public string RefundId { get; set; }
        public string Status { get; set; }
    }

}
