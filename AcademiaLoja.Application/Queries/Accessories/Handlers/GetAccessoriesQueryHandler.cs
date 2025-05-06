using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Dtos;
using AcademiaLoja.Application.Models.Responses.Acessory;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Queries.Accessories.Handlers
{
    public class GetAccessoriesQueryHandler : IRequestHandler<GetAccessoriesQuery, Result<IEnumerable<AccessoryResponse>>>
    {
        private readonly IAccessoryRepository _accessoriesRepository;
        public GetAccessoriesQueryHandler(IAccessoryRepository accessoriesRepository)
        {
            _accessoriesRepository = accessoriesRepository;
        }
        public async Task<Result<IEnumerable<AccessoryResponse>>> Handle(GetAccessoriesQuery query, CancellationToken cancellationToken)
        {
            var result = new Result<IEnumerable<AccessoryResponse>>();

            try
            {
                var accessoriess = await _accessoriesRepository.Get(query.Filter);

                // Aplicar filtros se necessário
                if (!accessoriess.Result(out var count).Any())
                {
                    result.WithError("Nenhum item encontrado.");
                    return result;
                }

                // Paginação
                var pagedItems = accessoriess.Result(out count);
                var response = new List<AccessoryResponse>();

                foreach (var accessories in pagedItems)
                {
                var responseItem = new AccessoryResponse
                {
                    Id = accessories.Id,
                    Name = accessories.Name,
                    Description = accessories.Description,
                    Color = accessories.Color,
                    Model = accessories.Model,
                    Size = accessories.Size                    
                };
                response.Add(responseItem);
                }

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
