using AcademiaLoja.Application.Models.Dtos;
using AcademiaLoja.Application.Models.Responses.SubCategories;

namespace AcademiaLoja.Application.Models.Responses.Categories
{
    public class CategoryResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Message { get; set; }
        public List<SubCategoryResponse> SubCategories { get; set; } = new List<SubCategoryResponse>();
    }
}
