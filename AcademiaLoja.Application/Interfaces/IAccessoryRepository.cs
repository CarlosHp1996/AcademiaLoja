using AcademiaLoja.Application.Models.Filters;
using AcademiaLoja.Application.Models.Requests.Accessoriess;
using AcademiaLoja.Application.Models.Responses.Acessory;
using AcademiaLoja.Domain.Entities;
using AcademiaLoja.Domain.Helpers;

namespace AcademiaLoja.Application.Interfaces
{
    public interface IAccessoryRepository : IBaseRepository<Accessory>
    {
        Task<AsyncOutResult<IEnumerable<Accessory>, int>> Get(GetAccessoriessRequestFilter filter);
        Task<AccessoryResponse> CreateAccessory(CreateAccessoryRequest request, CancellationToken cancellationToken);
    }
}
