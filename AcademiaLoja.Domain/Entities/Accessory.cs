using AcademiaLoja.Domain.Enums;

namespace AcademiaLoja.Domain.Entities
{
    public class Accessory
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public EnumColor Color { get; set; }
        public EnumModel Model { get; set; }
        public EnumSize Size { get; set; }       
    }
}
