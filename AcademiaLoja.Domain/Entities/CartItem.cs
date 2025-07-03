using AcademiaLoja.Domain.Enums;

namespace AcademiaLoja.Domain.Entities
{
    public class CartItem
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public EnumFlavor? Flavor { get; set; }
        public string? Size { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity;
        public DateTime AddedAt { get; set; }
    }
}
