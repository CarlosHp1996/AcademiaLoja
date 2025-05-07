using AcademiaLoja.Application.Models.Dtos;
using AcademiaLoja.Application.Models.Responses.Brands;
using AcademiaLoja.Domain.Enums;

namespace AcademiaLoja.Application.Models.Responses.Acessory
{
    public class AccessoryResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public EnumColor Color { get; set; }
        public EnumModel Model { get; set; }
        public EnumSize Size { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
