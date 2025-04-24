namespace AcademiaLoja.Domain.Entities
{
    public class Brand
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // Navegação
        public virtual ICollection<ProductBrand> ProductBrands { get; private set; } = new List<ProductBrand>();
    }
}
