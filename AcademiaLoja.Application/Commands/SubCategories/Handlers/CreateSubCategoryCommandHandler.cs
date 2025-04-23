using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Responses.SubCategories;
using AcademiaLoja.Domain.Entities;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.SubCategories.Handlers
{
    public class CreateSubCategoryCommandHandler : IRequestHandler<CreateSubCategoryCommand, Result<SubCategoryResponse>>
    {
        private readonly ISubCategoryRepository _subCategoryRepository;

        public CreateSubCategoryCommandHandler(ISubCategoryRepository subCategoryRepository)
        {
            _subCategoryRepository = subCategoryRepository;
        }
        public async Task<Result<SubCategoryResponse>> Handle(CreateSubCategoryCommand request, CancellationToken cancellationToken)
        {
            var result = new Result<SubCategoryResponse>();

            try
            {
                var subCategory = new SubCategory
                {
                    Name = request.Request.Name,
                    Description = request.Request.Description
                };
                _ = await _subCategoryRepository.AddAsync(subCategory);

                var response = new SubCategoryResponse
                {
                    Id = subCategory.Id,
                    Name = subCategory.Name,
                    Description = subCategory.Description
                };

                result.Value = response;
                result.Count = 1;
                result.HasSuccess = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro:", ex);
            }
            return result;
        }
    }
}
