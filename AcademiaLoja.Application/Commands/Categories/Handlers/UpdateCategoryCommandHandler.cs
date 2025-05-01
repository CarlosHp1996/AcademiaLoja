using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Responses.Categories;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Categories.Handlers
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result<CategoryResponse>>
    {
        private readonly ICategoryRepository _categoryRepository;

        public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<Result<CategoryResponse>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var result = new Result<CategoryResponse>();

            try
            {
                // Verificar se a categoria existe
                var category = await _categoryRepository.GetById(request.Id);

                if (category == null)
                {
                    result.WithError("Category not found.");
                    return result;
                }

                // Delegar toda a lógica de atualização para o repositório
                var response = await _categoryRepository.UpdateCategory(category, request.Request, cancellationToken);

                result.Value = response;
                result.HasSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.WithError($"Error updating category: {ex.Message}");
                return result;
            }
        }
    }
}
