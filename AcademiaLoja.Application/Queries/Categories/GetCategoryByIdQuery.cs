using AcademiaLoja.Application.Models.Responses.Categories;
using AcademiaLoja.Domain.Helpers;
using MediatR;


namespace AcademiaLoja.Application.Queries.Categories
{
    public class GetCategoryByIdQuery : IRequest<Result<CategoryResponse>>
    {
        public Guid Id { get; set; }
        public GetCategoryByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
