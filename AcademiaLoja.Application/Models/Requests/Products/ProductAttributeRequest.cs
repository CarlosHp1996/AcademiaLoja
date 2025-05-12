using AcademiaLoja.Domain.Enums;

namespace AcademiaLoja.Application.Models.Requests.Products
{
    public class ProductAttributeRequest
    {
        public string? Key { get; set; }
        public string? Value { get; set; }
        public EnumFlavor? Flavor { get; set; }
        public EnumBrand? Brand { get; set; }
        public EnumAccessory? Accessory { get; set; }
        public EnumCategory? Category { get; set; }
        public EnumObjective? Objective { get; set; }
    }
}
