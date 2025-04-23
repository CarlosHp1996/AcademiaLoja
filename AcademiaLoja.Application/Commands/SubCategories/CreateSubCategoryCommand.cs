using AcademiaLoja.Application.Models.Requests.SubCategories;
using AcademiaLoja.Application.Models.Responses.SubCategories;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.SubCategories
{
    public class CreateSubCategoryCommand : IRequest<Result<SubCategoryResponse>>
    {
        public CreateSubCategoryRequest Request;
        public CreateSubCategoryCommand(CreateSubCategoryRequest request)
        {
            Request = request;
        }
    }
}
