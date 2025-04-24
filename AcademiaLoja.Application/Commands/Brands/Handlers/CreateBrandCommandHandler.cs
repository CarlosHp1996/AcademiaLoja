using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Responses.Brands;
using AcademiaLoja.Domain.Entities;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Brands.Handlers
{
    public class CreateBrandCommandHandler : IRequestHandler<CreateBrandCommand, Result<BrandResponse>>
    {
        private readonly IBrandRepository _brandRepository;
        public CreateBrandCommandHandler(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }
        public async Task<Result<BrandResponse>> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
        {
            var result = new Result<BrandResponse>();

            try
            {
                var brand = new Brand()
                {
                    Name = request.Request.Name,
                    Description = request.Request.Description
                };

                _ = await _brandRepository.AddAsync(brand);

                var response = new BrandResponse
                {
                    Id = brand.Id,
                    Name = brand.Name,
                    Description = brand.Description
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
