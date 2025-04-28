namespace AcademiaLoja.Application.Models.Filters
{
    public class GetObjectivesRequestFilter : BaseRequestFilter
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
