using AcademiaLoja.Domain.Enums;

namespace AcademiaLoja.Domain.Entities
{
    public class ProductAttribute
    {
        public Guid Id { get;  set; }
        public Guid ProductId { get;  set; }
        public string? Key { get;  set; }
        public string? Value { get;  set; }
        public EnumFlavor? Flavor { get; set; }
        public EnumBrand? Brand { get; set; }
        public EnumAccessory? Accessory { get; set; }
        public EnumCategory? Category { get; set; }
        public EnumObjective? Objective { get; set; }

        // Navegação
        public virtual Product Product { get;  set; }
    }
}
