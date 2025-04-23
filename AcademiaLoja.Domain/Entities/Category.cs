namespace AcademiaLoja.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // Navegação
        public virtual ICollection<ProductCategory> ProductCategories { get; private set; } = new List<ProductCategory>();
        public virtual ICollection<CategorySubCategory> CategorySubCategories { get; set; } = new List<CategorySubCategory>();
    }
}
