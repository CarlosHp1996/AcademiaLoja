using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Responses.SubCategories;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Queries.SubCategories.Handlers
{
    public class GetSubCategoriesQueryHandler : IRequestHandler<GetSubCategoriesQuery, Result<IEnumerable<SubCategoryResponse>>>
    {
        private readonly ISubCategoryRepository _subCategoryRepository;

        public GetSubCategoriesQueryHandler(ISubCategoryRepository subCategoryRepository)
        {
            _subCategoryRepository = subCategoryRepository;
        }

        public async Task<Result<IEnumerable<SubCategoryResponse>>> Handle(GetSubCategoriesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = new Result<IEnumerable<SubCategoryResponse>>();
                var subCategories = await _subCategoryRepository.GetAll(request.filter.Take, request.filter.Offset, request.filter.SortingProp, request.filter.Ascending);

                if (!subCategories.Result(out var count).Any())
                {
                    result.WithError("Nenhuma subcategoria encontrada.");
                    return result;
                }

                var subCategoryList = subCategories.Result(out count);
                var response = new List<SubCategoryResponse>();

                foreach (var subCategory in subCategoryList)
                {
                    var responseItem = new SubCategoryResponse
                    {
                        Id = subCategory.Id,
                        Name = subCategory.Name,
                        Description = subCategory.Description
                    };
                    response.Add(responseItem);
                }

                result.Value = response;
                result.Count = count;
                result.Message = "Subcategorias listadas com sucesso";
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao manipular o GetSubCategoriesQuery.", ex);
            }
        }
    }
}
