using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Responses.SubCategories;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Queries.SubCategories.Handlers
{
    public class GetSubCategoryByIdQueryHandler : IRequestHandler<GetSubCategoryByIdQuery, Result<SubCategoryResponse>>
    {
        private readonly ISubCategoryRepository _subCategoryRepository;

        public GetSubCategoryByIdQueryHandler(ISubCategoryRepository subCategoryRepository)
        {
            _subCategoryRepository = subCategoryRepository;
        }

        public async Task<Result<SubCategoryResponse>> Handle(GetSubCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = new Result<SubCategoryResponse>();
                var subCategory = await _subCategoryRepository.GetById(request.Id);

                if (subCategory is null)
                {
                    result.WithError("Subcategoria não encontrada.");
                    return result;
                }

                var response = new SubCategoryResponse()
                {
                    Id = subCategory.Id,
                    Name = subCategory.Name,
                    Description = subCategory.Description
                };

                result.Count = 1;
                result.Message = "Subcategoria listada com sucesso!";
                result.HasSuccess = true;
                result.Value = response;
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro:", ex);
            }
        }
    }
}
