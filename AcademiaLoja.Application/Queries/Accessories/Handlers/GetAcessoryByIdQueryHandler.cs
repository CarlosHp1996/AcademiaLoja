using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Responses.Acessory;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Queries.Accessories.Handlers
{
    public class GetAcessoryByIdQueryHandler : IRequestHandler<GetAcessoryByIdQuery, Result<AccessoryResponse>>
    {
        private readonly IAccessoryRepository _accessoriesRepository;
        public GetAcessoryByIdQueryHandler(IAccessoryRepository accessoriesRepository)
        {
            _accessoriesRepository = accessoriesRepository;
        }
        public async Task<Result<AccessoryResponse>> Handle(GetAcessoryByIdQuery request, CancellationToken cancellationToken)
        {
            var result = new Result<AccessoryResponse>();

            try
            {
                var accessories = await _accessoriesRepository.GetById(request.Id);

                if (accessories == null)
                {
                    result.HasSuccess = false;
                    result.Message = "Accessories não encontrado.";
                    return result;
                }

                var response = new AccessoryResponse
                {
                    Id = accessories.Id,
                    Name = accessories.Name,
                    Description = accessories.Description,
                    Color = accessories.Color,
                    Model = accessories.Model,
                    Size = accessories.Size
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
