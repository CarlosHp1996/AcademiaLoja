using AcademiaLoja.Application.Models.Requests.Categories;
using AcademiaLoja.Application.Models.Responses.Categories;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Categories
{
    public class UpdateCategoryCommand : IRequest<Result<CategoryResponse>>
    {
        public Guid Id { get; set; }
        public UpdateCategoryRequest Request { get; set; }
        public UpdateCategoryCommand(Guid id, UpdateCategoryRequest request)
        {
            Id = id;
            Request = request;
        }
    }
}
