namespace AcademiaLoja.Domain.Entities
{
    public class OrderItem
    {
        public Guid Id { get; private set; }
        public Guid OrderId { get; private set; }
        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }

        // Navegação
        public virtual Order Order { get; private set; }
        public virtual Product Product { get; private set; }
    }
}
