using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Responses.Trackings;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Queries.Trackings.Handlers
{
    public class GetTrackingsQueryHandler : IRequestHandler<GetTrackingsQuery, Result<List<TrackingResponse>>>
    {
        private readonly ITrackingRepository _trackingRepository;

        public GetTrackingsQueryHandler(ITrackingRepository trackingRepository)
        {
            _trackingRepository = trackingRepository;
        }

        public async Task<Result<List<TrackingResponse>>> Handle(GetTrackingsQuery request, CancellationToken cancellationToken)
        {
            var result = new Result<List<TrackingResponse>>();

            try
            {
                // Delega toda a lógica de filtros e paginação para o repositório
                var (trackingResponses, totalCount) = await _trackingRepository.GetTrackingsFilteredAsync(
                    request.Filter.Page,
                    request.Filter.PageSize,
                    request.Filter.SortingProp,
                    request.Filter.Ascending,
                    request.Filter.Status,
                    request.Filter.OrderId,
                    request.Filter.TrackingNumber,
                    request.Filter.Description,
                    request.Filter.Location,
                    request.Filter.StartDate,
                    request.Filter.EndDate,
                    cancellationToken);

                result.Value = trackingResponses;
                result.Count = trackingResponses.Count;
                result.Count = totalCount;
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