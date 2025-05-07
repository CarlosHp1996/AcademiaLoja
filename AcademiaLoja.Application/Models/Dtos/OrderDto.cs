namespace AcademiaLoja.Application.Models.Dtos
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public string TrackingNumber { get; set; }
        public string ShippingAddress { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
        public List<PaymentDto> Payments { get; set; } = new List<PaymentDto>();
    }
}
