using System.ComponentModel.DataAnnotations;

namespace AcademiaLoja.Application.Models.Dtos
{
    public class ProductAttributeDto
    {
        public Guid Id { get; set; }
        [Required]
        public string Key { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
