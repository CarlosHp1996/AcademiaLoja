namespace AcademiaLoja.Application.Models.Requests.Categories
{
    public class UpdateCategoryRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<Guid>? SubCategoryIds { get; set; } = new List<Guid>();
    }
}
