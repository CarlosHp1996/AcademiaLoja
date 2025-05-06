namespace AcademiaLoja.Application.Models.Filters
{
    public class GetProductsRequestFilter : BaseRequestFilter
    {
        // Filtros
        public string? Name { get; set; } // Filtro por nome do produto
        public List<Guid>? CategoryIds { get; set; } = new List<Guid>(); // Filtro por categorias
        public List<int>? QuantityRanges { get; set; } = new List<int>(); // Filtro por faixas de quantidade
        public List<string>? Flavors { get; set; } = new List<string>(); // Filtro por sabores
        public List<string>? BrandIds { get; set; } = new List<string>(); // Filtro por marcas
        public List<string>? ObjectiveIds { get; set; } = new List<string>(); // Filtro por objetivos
        public List<string>? AccessoryIds { get; set; } = new List<string>(); // Filtro por acessórios
        public decimal? MinPrice { get; set; } // Preço mínimo
        public decimal? MaxPrice { get; set; } // Preço máximo
        public bool? IsActive { get; set; } // Filtro por status (ativo/inativo)
    }
}
