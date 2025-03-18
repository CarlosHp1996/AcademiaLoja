using AcademiaLoja.Domain.Entities.Security;

namespace AcademiaLoja.Domain.Entities
{
    public class Order
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }
        public decimal TotalAmount { get; private set; }
        public string Status { get; private set; }
        public string PaymentStatus { get; private set; }
        public string TrackingNumber { get; private set; }
        public string ShippingAddress { get; private set; }
        public DateTime OrderDate { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        // Navegação
        public virtual ApplicationUser User { get; private set; }
        public virtual ICollection<OrderItem> OrderItems { get; private set; } = new List<OrderItem>();
        public virtual ICollection<Payment> Payments { get; private set; } = new List<Payment>();
    }
}
