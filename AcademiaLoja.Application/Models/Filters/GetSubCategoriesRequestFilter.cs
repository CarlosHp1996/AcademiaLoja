namespace AcademiaLoja.Application.Models.Filters
{
    public class GetSubCategoriesRequestFilter : BaseRequestFilter
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
