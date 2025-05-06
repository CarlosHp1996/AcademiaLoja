using AcademiaLoja.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace AcademiaLoja.Application.Models.Requests.Accessoriess
{
    public class CreateAccessoryRequest
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
        public EnumColor Color { get; set; }
        public EnumModel Model { get; set; }
        public EnumSize Size { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero.")]
        public decimal Price { get; set; }
        public List<Guid> BrandIds { get; set; } = new List<Guid>();

    }
}
