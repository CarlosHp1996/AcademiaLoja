using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Responses.Trackings;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Queries.Trackings.Handlers
{
    public class GetTrackingByIdQueryHandler : IRequestHandler<GetTrackingByIdQuery, Result<TrackingResponse>>
    {
        private readonly ITrackingRepository _trackingRepository;

        public GetTrackingByIdQueryHandler(ITrackingRepository trackingRepository)
        {
            _trackingRepository = trackingRepository;
        }

        public async Task<Result<TrackingResponse>> Handle(GetTrackingByIdQuery request, CancellationToken cancellationToken)
        {
            var result = new Result<TrackingResponse>();

            try
            {
                var tracking = await _trackingRepository.GetById(request.Id);

                if (tracking == null)
                {
                    result.WithError("Tracking não encontrado");
                    return result;
                }

                var response = new TrackingResponse
                {
                    Id = tracking.Id,
                    OrderId = tracking.OrderId,
                    Status = tracking.Status,
                    Description = tracking.Description,
                    Location = tracking.Location,
                    EventDate = tracking.EventDate,
                    CreatedAt = tracking.CreatedAt,
                    TrackingNumber = tracking.TrackingNumber
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