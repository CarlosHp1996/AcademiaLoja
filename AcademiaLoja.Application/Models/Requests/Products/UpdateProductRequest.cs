using AcademiaLoja.Application.Models.Dtos;
using System.ComponentModel.DataAnnotations;

namespace AcademiaLoja.Application.Models.Requests.Products
{
    public class UpdateProductRequest
    {

        [StringLength(100)]
        public string? Name { get; set; }

        public string? Description { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal? Price { get; set; }

        [Range(0, int.MaxValue)]
        public int? StockQuantity { get; set; }

        public string? ImageUrl { get; set; }

        public bool? IsActive { get; set; }

        public List<Guid> CategoryIds { get; set; } = new List<Guid>();

        // Atributos do produto (sabor, marca, etc.)
        public List<ProductAttributeDto> Attributes { get; set; } = new List<ProductAttributeDto>();
    }
}
