namespace AcademiaLoja.Domain.Entities
{
    public class Inventory
    {
        public Guid Id { get; private set; }
        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }
        public DateTime LastUpdated { get; private set; }

        // Navegação
        public virtual Product Product { get; private set; }
    }
}
