using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Responses.Brands;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Queries.Brands.Handlers
{
    public class GetBrandsQueryHandler : IRequestHandler<GetBrandsQuery, Result<IEnumerable<BrandResponse>>>
    {
        private readonly IBrandRepository _brandRepository;

        public GetBrandsQueryHandler(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public async Task<Result<IEnumerable<BrandResponse>>> Handle(GetBrandsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = new Result<IEnumerable<BrandResponse>>();
                //ALTERAR E CRIAR UM GET PERSONALIZADO IGUAL DO PRODUCT
                var brands = await _brandRepository.GetAll(request.Filter.Take, request.Filter.Offset, request.Filter.SortingProp, request.Filter.Ascending);

                if (!brands.Result(out var count).Any())
                {
                    result.WithError("Nenhuma marca encontrada.");
                    return result;
                }

                var brandList = brands.Result(out count);

                var response = new List<BrandResponse>();
                foreach (var brand in brandList)
                {
                    var responseItem = new BrandResponse
                    {
                        Id = brand.Id,
                        Name = brand.Name,
                        Description = brand.Description
                    };
                    response.Add(responseItem);
                }
                result.Value = response;
                result.Count = count;
                result.Message = "Marcas listadas com sucesso";
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao manipular o GetBrandsQuery.", ex);
            }
        }
    }
}
