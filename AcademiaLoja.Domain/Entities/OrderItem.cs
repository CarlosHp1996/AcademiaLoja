namespace AcademiaLoja.Domain.Entities
{
    public class OrderItem
    {
        public int Id { get; private set; }
        public int OrderId { get; private set; }
        public int ProductId { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }

        // Navegação
        public virtual Order Order { get; private set; }
        public virtual Product Product { get; private set; }
    }
}
