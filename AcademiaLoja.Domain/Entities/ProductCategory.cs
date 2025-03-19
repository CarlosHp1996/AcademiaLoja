namespace AcademiaLoja.Domain.Entities
{
    public class ProductCategory
    {
        public Guid ProductId { get; private set; }
        public Guid CategoryId { get; private set; }

        // Navegação
        public virtual Product Product { get; private set; }
        public virtual Category Category { get; private set; }
    }
}
