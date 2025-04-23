using AcademiaLoja.Application.Models.Filters;
using AcademiaLoja.Application.Models.Responses.SubCategories;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Queries.SubCategories
{
    public class GetSubCategoriesQuery : IRequest<Result<IEnumerable<SubCategoryResponse>>>
    {
        public GetSubCategoriesRequestFilter filter;
        public GetSubCategoriesQuery(GetSubCategoriesRequestFilter filter)
        {
            this.filter = filter;
        }
    }
}
