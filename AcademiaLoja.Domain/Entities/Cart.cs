namespace AcademiaLoja.Domain.Entities
{
    public class Cart
    {
        public required string UserId { get; set; } // Pode ser sessionId para usuários não logados
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public decimal TotalAmount => Items.Sum(i => i.TotalPrice);
        public int TotalItems => Items.Sum(i => i.Quantity);
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
