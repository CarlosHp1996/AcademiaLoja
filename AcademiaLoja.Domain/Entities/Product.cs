namespace AcademiaLoja.Domain.Entities
{
    public class Product
    {
        public Guid Id { get;  set; }
        public string Name { get;  set; }
        public string Description { get;  set; }
        public decimal Price { get;  set; }
        public int StockQuantity { get;  set; }
        public string ImageUrl { get; set; }
        public bool IsActive { get;  set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get;  set; }

        // Navegação
        public virtual ICollection<ProductCategory> ProductCategories { get;  set; } = new List<ProductCategory>();
        public virtual ICollection<OrderItem> OrderItems { get;  set; } = new List<OrderItem>();
        public virtual ICollection<ProductAttribute> Attributes { get;  set; } = new List<ProductAttribute>();
        public virtual Inventory Inventory { get;  set; }
        public virtual ICollection<ProductBrand> ProductBrands { get; set; } = new List<ProductBrand>();
        public virtual ICollection<ProductObjective> ProductObjectives { get; private set; } = new List<ProductObjective>();
    }
}
