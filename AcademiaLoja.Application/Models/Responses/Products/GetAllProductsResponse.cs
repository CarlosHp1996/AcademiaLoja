using AcademiaLoja.Application.Models.Dtos;

namespace AcademiaLoja.Application.Models.Responses.Products
{
    public class GetAllProductsResponse
    {
        public List<ProductListItemDto>? Products { get; set; } = new List<ProductListItemDto>();
        public PaginationDto? Pagination { get; set; }
        public FiltersDto? Filters { get; set; }
    }
}
