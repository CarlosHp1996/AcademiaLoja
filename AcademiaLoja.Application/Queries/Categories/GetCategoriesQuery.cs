using AcademiaLoja.Application.Models.Filters;
using AcademiaLoja.Application.Models.Responses.Categories;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Queries.Categories
{
    public class GetCategoriesQuery : IRequest<Result<IEnumerable<CategoryResponse>>>
    {
        public GetCategoriesRequestFilter filter;

        public GetCategoriesQuery(GetCategoriesRequestFilter filter)
        {
            this.filter = filter;
        }
    }
}
