using AcademiaLoja.Domain.Enums;

namespace AcademiaLoja.Application.Models.Dtos
{
    public class ProductAttributeDto
    {
        public Guid? Id { get; set; }
        public string? Key { get; set; }
        public string? Value { get; set; }
    }
}
