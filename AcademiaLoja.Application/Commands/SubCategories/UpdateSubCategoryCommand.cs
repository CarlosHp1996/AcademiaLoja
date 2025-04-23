using AcademiaLoja.Application.Models.Requests.SubCategories;
using AcademiaLoja.Application.Models.Responses.SubCategories;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.SubCategories
{
    public class UpdateSubCategoryCommand : IRequest<Result<SubCategoryResponse>>
    {
        public Guid Id { get; set; }
        public UpdateSubCategoryRequest Request { get; set; }
        public UpdateSubCategoryCommand(Guid id, UpdateSubCategoryRequest request)
        {
            Id = id;
            Request = request;
        }
    }
}
