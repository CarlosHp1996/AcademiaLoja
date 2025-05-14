using Microsoft.AspNetCore.Http;
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

        public IFormFile? ImageUrl { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        // Atributos do produto (sabor, marca, etc.)
        public List<ProductAttributeRequest> Attributes { get; set; } = new List<ProductAttributeRequest>();
    }
}
