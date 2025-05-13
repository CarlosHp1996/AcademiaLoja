using AcademiaLoja.Application.Models.Requests.Trackings;
using AcademiaLoja.Application.Models.Responses.Trackings;
using AcademiaLoja.Domain.Entities;

namespace AcademiaLoja.Application.Interfaces
{
    public interface ITrackingRepository : IBaseRepository<Tracking>
    {
        Task<TrackingResponse> CreateTrackingEventAsync(CreateTrackingRequest eventData);

        Task<TrackingResponse> UpdateTrackingAsync(Tracking tracking, UpdateTrackingRequest request, CancellationToken cancellationToken);

        // Novo método que encapsula toda a lógica de filtros e paginação
        Task<(List<TrackingResponse> Trackings, int TotalCount)> GetTrackingsFilteredAsync(
            int? page,
            int? pageSize,
            string sortingProp,
            bool ascending,
            string status = null,
            Guid? orderId = null,
            string trackingNumber = null,
            string description = null,
            string location = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            CancellationToken cancellationToken = default);
    }
}