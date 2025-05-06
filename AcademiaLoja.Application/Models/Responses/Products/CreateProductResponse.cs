using AcademiaLoja.Application.Models.Dtos;

namespace AcademiaLoja.Application.Models.Responses.Products
{
    public class CreateProductResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
        public List<BrandDto> Brands { get; set; } = new List<BrandDto>();
        public List<ProductAttributeDto> Attributes { get; set; } = new List<ProductAttributeDto>();
        public List<AccessoryDto> Accessories { get; set; } = new List<AccessoryDto>();
        public string Message { get; set; }
    }   
}
