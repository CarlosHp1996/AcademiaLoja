using AcademiaLoja.Application.Models.Dtos;
using System.ComponentModel.DataAnnotations;

namespace AcademiaLoja.Application.Models.Requests.Products
{
    public class CreateProductRequest
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        public string ImageUrl { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public List<Guid> CategoryIds { get; set; } = new List<Guid>();

        // Atributos do produto (sabor, marca, etc.)
        public List<ProductAttributeRequest> Attributes { get; set; } = new List<ProductAttributeRequest>();

        [Required]
        public List<Guid> BrandIds { get; set; } = new List<Guid>();

        [Required]
        public List<Guid> ObjectivesId { get; set; } = new List<Guid>();
    }
}
