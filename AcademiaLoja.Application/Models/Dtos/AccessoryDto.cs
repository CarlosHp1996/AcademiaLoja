using AcademiaLoja.Domain.Enums;

namespace AcademiaLoja.Application.Models.Dtos
{
    public class AccessoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public EnumColor Color { get; set; }
        public EnumModel Model { get; set; }
        public EnumSize Size { get; set; }
    }
}
