using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Responses.Brands;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Queries.Brands.Handlers
{
    public class GetBrandByIdQueryHandler : IRequestHandler<GetBrandByIdQuery, Result<BrandResponse>>
    {
        private readonly IBrandRepository _brandRepository;

        public GetBrandByIdQueryHandler(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public async Task<Result<BrandResponse>> Handle(GetBrandByIdQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var result = new Result<BrandResponse>();
                //Criar getbyid com filtros e verificar os outros
                var brand = await _brandRepository.GetById(query.Id);

                if (brand is null)
                {
                    result.WithNotFound("Marca não encontrada!");
                    return result;
                }

                var response = new BrandResponse
                {
                    Id = brand.Id,
                    Name = brand.Name,
                    Description = brand.Description
                };

                result.Value = response;
                result.Count = 1;
                result.Message = "Marca listada com sucesso";
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao listar a marca", ex);
            }
        }
    }
}
