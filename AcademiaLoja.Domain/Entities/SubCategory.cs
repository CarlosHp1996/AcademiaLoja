namespace AcademiaLoja.Domain.Entities
{
    public class SubCategory
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // Navegação
        public virtual ICollection<CategorySubCategory> CategorySubCategories { get; private set; } = new List<CategorySubCategory>();
    }
}
