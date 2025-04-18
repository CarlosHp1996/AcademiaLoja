namespace AcademiaLoja.Domain.Entities
{
    public class ProductCategory
    {
        public Guid ProductId { get;  set; }
        public Guid CategoryId { get;  set; }

        // Navegação
        public virtual Product Product { get;  set; }
        public virtual Category Category { get;  set; }
    }
}
