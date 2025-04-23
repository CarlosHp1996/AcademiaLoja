using AcademiaLoja.Application.Models.Responses.SubCategories;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Queries.SubCategories
{
    public class GetSubCategoryByIdQuery : IRequest<Result<SubCategoryResponse>>
    {
        public Guid Id { get; set; }
        public GetSubCategoryByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
