using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Responses.Categories;
using AcademiaLoja.Domain.Entities;
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
                var category = new Category
                {
                    Name = request.Request.Name,
                    Description = request.Request.Description
                };
                _ = await _categoryRepository.AddAsync(category);

                var response = new CategoryResponse
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description
                };

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
