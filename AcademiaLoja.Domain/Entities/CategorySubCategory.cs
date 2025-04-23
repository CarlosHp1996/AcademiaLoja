namespace AcademiaLoja.Domain.Entities
{
    public class CategorySubCategory
    {
        public Guid CategoryId { get; set; }
        public Guid SubCategoryId { get; set; }


        // Navegação
        public virtual Category Category { get; set; }
        public virtual SubCategory SubCategory { get; set; }
        
    }
}
