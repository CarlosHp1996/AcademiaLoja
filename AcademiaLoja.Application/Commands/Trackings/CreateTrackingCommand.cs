using AcademiaLoja.Application.Models.Dtos;
using AcademiaLoja.Application.Models.Requests.Trackings;
using AcademiaLoja.Application.Models.Responses.Trackings;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Trackings
{
    public class CreateTrackingCommand : IRequest<Result<TrackingResponse>>
    {
        public CreateTrackingRequest Request;
        public CreateTrackingCommand(CreateTrackingRequest request)
        {
            Request = request;
            
        }
    }
}
