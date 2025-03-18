namespace AcademiaLoja.Domain.Entities
{
    public class ProductCategory
    {
        public int ProductId { get; private set; }
        public int CategoryId { get; private set; }

        // Navegação
        public virtual Product Product { get; private set; }
        public virtual Category Category { get; private set; }
    }
}
