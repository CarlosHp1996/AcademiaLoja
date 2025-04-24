using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Responses.Categories;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Queries.Categories.Handlers
{
    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, Result<CategoryResponse>>
    {
        private readonly ICategoryRepository _categoryRepository;

        public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<Result<CategoryResponse>> Handle(GetCategoryByIdQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var result = new Result<CategoryResponse>();
                var category = await _categoryRepository.GetById(query.Id);

                if (category is null)
                {
                    result.WithNotFound("Categoria não encontrada!");
                    return result;
                }

                var response = new CategoryResponse
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description
                };

                result.Value = response;
                result.Count = 1;
                result.Message = "Categoria listada com sucesso";
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao listar a categoria", ex);
            }
        }
    }
}
