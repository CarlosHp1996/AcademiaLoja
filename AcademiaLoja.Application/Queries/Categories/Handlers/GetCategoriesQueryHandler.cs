using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Responses.Categories;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Queries.Categories.Handlers
{
    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, Result<IEnumerable<CategoryResponse>>>
    {
        private readonly ICategoryRepository _categoryRepository;

        public GetCategoriesQueryHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Result<IEnumerable<CategoryResponse>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = new Result<IEnumerable<CategoryResponse>>();
                //ALTERAR E CRIAR UM GET PERSONALIZADO IGUAL DO PRODUCT
                var categories = await _categoryRepository.GetAll(request.filter.Take, request.filter.Offset, request.filter.SortingProp, request.filter.Ascending);

                if (!categories.Result(out var count).Any())
                {
                    result.WithError("Nenhuma categoria encontrada.");
                    return result;
                }

                var categoryList = categories.Result(out count);             

               var response = new List<CategoryResponse>();
                foreach (var category in categoryList)
                {
                    var responseItem = new CategoryResponse
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Description = category.Description
                    };
                    response.Add(responseItem);
                }
                result.Value = response;
                result.Count = count;
                result.Message = "Categorias listadas com sucesso";
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao manipular o GetCategoriesQuery.", ex);
            }
        }
    }
}
