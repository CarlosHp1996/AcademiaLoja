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
            try
            {
                var result = new Result<CategoryResponse>();
                var category = await _categoryRepository.GetById(request.Id);

                if (category is null)
                {
                    result.WithError("Categoria não encontrada.");
                    return result;
                }

                category.Id = request.Id;
                category.Name = request.Request.Name;
                category.Description = request.Request.Description;
                var update = await _categoryRepository.UpdateAsync(category);

                if (!update)
                {
                    result.WithError("Erro ao atualizar a categoria.");
                    return result;
                }

                var response = new CategoryResponse()
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description
                };

                result.Count = 1;
                result.Message = "Categoria alterada com sucesso!";
                result.HasSuccess = true;
                result.Value = response;
                return result;
            }
            catch (Exception)
            {

                throw;
            }          

        }
    }
}
