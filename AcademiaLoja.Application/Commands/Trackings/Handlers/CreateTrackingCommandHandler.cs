using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Responses.Trackings;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Trackings.Handlers
{
    public class CreateTrackingCommandHandler : IRequestHandler<CreateTrackingCommand, Result<TrackingResponse>>
    {
        private readonly ITrackingRepository _trackingRepository;

        public CreateTrackingCommandHandler(ITrackingRepository trackingRepository)
        {
            _trackingRepository = trackingRepository;
        }

        public async Task<Result<TrackingResponse>> Handle(CreateTrackingCommand request, CancellationToken cancellationToken)
        {
            var result = new Result<TrackingResponse>();

            try
            {
                // Adicionar o evento de rastreamento
                var trackingInfo = await _trackingRepository.CreateTrackingEventAsync(request.Request);
               
                if (trackingInfo == null)
                {
                    result.WithError("Não foi possível recuperar as informações de rastreio após a criação");
                    return result;
                }

                result.Value = trackingInfo;
                result.Count = 1;
                result.HasSuccess = true;
            }
            catch (KeyNotFoundException ex)
            {
                result.WithError(ex.Message);
            }
            catch (Exception ex)
            {
                result.WithError($"Erro ao criar evento de rastreio: {ex.Message}");
            }

            return result;
        }
    }
}