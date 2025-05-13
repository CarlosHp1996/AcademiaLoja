using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Responses.Trackings;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Trackings.Handlers
{
    public class UpdateTrackingCommandHandler : IRequestHandler<UpdateTrackingCommand, Result<TrackingResponse>>
    {
        private readonly ITrackingRepository _trackingRepository;

        public UpdateTrackingCommandHandler(ITrackingRepository trackingRepository)
        {
            _trackingRepository = trackingRepository;
        }

        public async Task<Result<TrackingResponse>> Handle(UpdateTrackingCommand request, CancellationToken cancellationToken)
        {
            var result = new Result<TrackingResponse>();

            try
            {
                // Buscar o tracking existente
                var tracking = await _trackingRepository.GetById(request.Id);
                if (tracking == null)
                {
                    result.WithError("Tracking não encontrado");
                    return result;
                }

                // Delegar toda a lógica de atualização para o repositório
                var response = await _trackingRepository.UpdateTrackingAsync(tracking, request.Request, cancellationToken);

                result.Value = response;
                result.HasSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.WithError(ex.Message);
            }

            return result;
        }
    }
}