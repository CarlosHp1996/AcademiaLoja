using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Categories.Handlers
{
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result>
    {
        private readonly ICategoryRepository _categoryRepository;

        public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = new Result();
                var category = await _categoryRepository.GetById(request.Id);

                if (category is null)
                {
                    result.WithNotFound("Categoria não encontrada!");
                    return result;
                }

                await _categoryRepository.DeleteAsync(category);

                result.Message = "Categoria excluída com sucesso!";
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
