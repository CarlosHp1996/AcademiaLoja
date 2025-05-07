namespace AcademiaLoja.Application.Models.Requests.Orders
{
    public class UpdateOrderRequest
    {
        public string? Status { get; set; }
        public string? PaymentStatus { get; set; }
        public string? TrackingNumber { get; set; }
        public string? ShippingAddress { get; set; }
    }
}
