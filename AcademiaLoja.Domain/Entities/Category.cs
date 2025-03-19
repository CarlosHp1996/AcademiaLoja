namespace AcademiaLoja.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }

        // Navegação
        public virtual ICollection<ProductCategory> ProductCategories { get; private set; } = new List<ProductCategory>();
    }
}
