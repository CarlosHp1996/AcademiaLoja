using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Responses.SubCategories;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.SubCategories.Handlers
{
    public class UpdateSubCategoryCommandHandler : IRequestHandler<UpdateSubCategoryCommand, Result<SubCategoryResponse>>
    {
        private readonly ISubCategoryRepository _subCategoryRepository;

        public UpdateSubCategoryCommandHandler(ISubCategoryRepository subCategoryRepository)
        {
            _subCategoryRepository = subCategoryRepository;
        }

        public async Task<Result<SubCategoryResponse>> Handle(UpdateSubCategoryCommand request, CancellationToken cancellationToken)
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

                subCategory.Id = request.Id;
                subCategory.Name = request.Request.Name;
                subCategory.Description = request.Request.Description;
                var update = await _subCategoryRepository.UpdateAsync(subCategory);

                if (!update)
                {
                    result.WithError("Erro ao atualizar a subcategoria.");
                    return result;
                }

                var response = new SubCategoryResponse()
                {
                    Id = subCategory.Id,
                    Name = subCategory.Name,
                    Description = subCategory.Description
                };

                result.Count = 1;
                result.Message = "Subcategoria alterada com sucesso!";
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
