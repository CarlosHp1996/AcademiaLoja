using AcademiaLoja.Application.Models.Responses.Trackings;
using AcademiaLoja.Domain.Helpers;
using MediatR;
using System;

namespace AcademiaLoja.Application.Queries.Trackings
{
    public class GetTrackingByIdQuery : IRequest<Result<TrackingResponse>>
    {
        public Guid Id { get; set; }

        public GetTrackingByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
