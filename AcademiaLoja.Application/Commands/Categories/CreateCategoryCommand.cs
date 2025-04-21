using AcademiaLoja.Application.Models.Requests.Categories;
using AcademiaLoja.Application.Models.Responses.Categories;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Categories
{
    public class CreateCategoryCommand : IRequest<Result<CategoryResponse>>
    {
        public CreateCategoryRequest Request;

        public CreateCategoryCommand(CreateCategoryRequest request)
        {
            Request = request;
        }
    }
}
