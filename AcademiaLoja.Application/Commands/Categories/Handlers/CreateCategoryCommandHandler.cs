using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Responses.Categories;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Categories.Handlers
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<CategoryResponse>>
    {
        private readonly ICategoryRepository _categoryRepository;

        public CreateCategoryCommandHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<Result<CategoryResponse>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var result = new Result<CategoryResponse>();

            try
            {
                var response = await _categoryRepository.CreateCategory(request.Request, cancellationToken);           
                result.Value = response;
                result.Count = 1;
                result.HasSuccess = true;                
            }
            catch (Exception)
            {

                throw;
            }
            return result;
        }
    }
}
