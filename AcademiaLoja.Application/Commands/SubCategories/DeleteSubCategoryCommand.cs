using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.SubCategories
{
    public class DeleteSubCategoryCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public DeleteSubCategoryCommand(Guid id)
        {
            Id = id;
        }
    }
}
