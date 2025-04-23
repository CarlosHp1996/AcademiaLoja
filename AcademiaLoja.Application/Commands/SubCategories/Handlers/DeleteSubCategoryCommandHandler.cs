using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.SubCategories.Handlers
{
    public class DeleteSubCategoryCommandHandler : IRequestHandler<DeleteSubCategoryCommand, Result>
    {
        private readonly ISubCategoryRepository _subCategoryRepository;
        public DeleteSubCategoryCommandHandler(ISubCategoryRepository subCategoryRepository)
        {
            _subCategoryRepository = subCategoryRepository;
        }
        public async Task<Result> Handle(DeleteSubCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = new Result();
                var subCategory = await _subCategoryRepository.GetById(request.Id);

                if (subCategory is null)
                {
                    result.WithNotFound("SubCategoria não encontrada!");
                    return result;
                }

                await _subCategoryRepository.DeleteAsync(subCategory);

                result.Message = "SubCategoria excluída com sucesso!";
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
