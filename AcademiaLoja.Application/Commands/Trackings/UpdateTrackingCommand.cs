using AcademiaLoja.Application.Models.Requests.Trackings;
using AcademiaLoja.Application.Models.Responses.Trackings;
using AcademiaLoja.Domain.Helpers;
using MediatR;
using System;

namespace AcademiaLoja.Application.Commands.Trackings
{
    public class UpdateTrackingCommand : IRequest<Result<TrackingResponse>>
    {
        public Guid Id { get; set; }
        public UpdateTrackingRequest Request;
        public UpdateTrackingCommand(Guid id, UpdateTrackingRequest request)
        {
            Id = id;
            Request = request;
        }
    }
}
