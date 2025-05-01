namespace AcademiaLoja.Application.Models.Requests.Categories
{
    public class CreateCategoryRequest
    {
        public List<Guid> SubCategoryIds { get; set; } = new List<Guid>();
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
