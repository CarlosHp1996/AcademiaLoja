using AcademiaLoja.Application.Models.Requests.Categories;
using AcademiaLoja.Application.Models.Responses.Categories;
using AcademiaLoja.Domain.Entities;

namespace AcademiaLoja.Application.Interfaces
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        Task<CategoryResponse> CreateCategory(CreateCategoryRequest request, CancellationToken cancellationToken);
        Task<CategoryResponse> UpdateCategory(Category category, UpdateCategoryRequest request, CancellationToken cancellationToken);
    }
}
