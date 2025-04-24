namespace AcademiaLoja.Domain.Entities
{
    public class ProductBrand
    {
        public Guid ProductId { get; set; }
        public Guid BrandId { get; set; }

        // Navegação
        public virtual Product Product { get; set; }
        public virtual Brand Brand { get; set; }
    }
}
