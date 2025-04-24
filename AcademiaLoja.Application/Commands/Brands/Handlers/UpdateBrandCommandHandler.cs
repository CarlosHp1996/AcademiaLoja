using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Responses.Brands;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Brands.Handlers
{
    public class UpdateBrandCommandHandler : IRequestHandler<UpdateBrandCommand, Result<BrandResponse>>
    {
        private readonly IBrandRepository _brandRepository;
        public UpdateBrandCommandHandler(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }
        public async Task<Result<BrandResponse>> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
        {
            var result = new Result<BrandResponse>();

            try
            {
                var brand = await _brandRepository.GetById(request.Id);

                if (brand is null)
                {
                    result.WithError("Marca não encontrada.");
                    return result;
                }

                brand.Name = request.Request.Name;
                brand.Description = request.Request.Description;
                var update = await _brandRepository.UpdateAsync(brand);

                if (!update)
                {
                    result.WithError("Erro ao atualizar a marca.");
                    return result;
                }

                var response = new BrandResponse()
                {
                    Id = brand.Id,
                    Name = brand.Name,
                    Description = brand.Description
                };

                result.Value = response;
                result.Count = 1;
                result.HasSuccess = true;
            }
            catch (Exception ex)
            {
                result.WithError(ex.Message);
            }

            return result;
        }
    }
}
