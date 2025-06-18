using AcademiaLoja.Domain.Enums;

namespace AcademiaLoja.Application.Models.Filters
{
    public class GetProductsRequestFilter : BaseRequestFilter
    {
        // Filtros
        public string? Name { get; set; } // Filtro por nome do produto
        public List<EnumCategory>? CategoryIds { get; set; } = new List<EnumCategory>(); // Filtro por categorias
        public List<int>? QuantityRanges { get; set; } = new List<int>(); // Filtro por faixas de quantidade
        public List<EnumFlavor>? Flavors { get; set; } = new List<EnumFlavor>(); // Filtro por sabores
        public List<EnumBrand>? BrandIds { get; set; } = new List<EnumBrand>(); // Filtro por marcas
        public List<EnumObjective>? ObjectiveIds { get; set; } = new List<EnumObjective>(); // Filtro por objetivos
        public List<EnumAccessory>? AccessoryIds { get; set; } = new List<EnumAccessory>(); // Filtro por acessórios
        public decimal? MinPrice { get; set; } // Preço mínimo
        public decimal? MaxPrice { get; set; } // Preço máximo
        public bool? IsActive { get; set; } // Filtro por status (ativo/inativo)
    }
}
