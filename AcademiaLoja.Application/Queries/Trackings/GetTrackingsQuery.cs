using AcademiaLoja.Application.Models.Filters;
using AcademiaLoja.Application.Models.Responses.Trackings;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Queries.Trackings
{
    public class GetTrackingsQuery : IRequest<Result<List<TrackingResponse>>>
    {
        public GetTrackingsRequestFilter Filter { get; set; }

        public GetTrackingsQuery(GetTrackingsRequestFilter filter)
        {
            Filter = filter;
        }
    }
}
