namespace AcademiaLoja.Application.Models.Filters
{
    public class GetCategoriesRequestFilter : BaseRequestFilter
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
