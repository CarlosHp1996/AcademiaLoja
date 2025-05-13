using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Trackings.Handlers
{
    public class DeleteTrackingCommandHandler : IRequestHandler<DeleteTrackingCommand, Result>
    {
        private readonly ITrackingRepository _trackingRepository;

        public DeleteTrackingCommandHandler(ITrackingRepository trackingRepository)
        {
            _trackingRepository = trackingRepository;
        }

        public async Task<Result> Handle(DeleteTrackingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = new Result();
                var tracking = await _trackingRepository.GetById(request.Id);

                if (tracking is null)
                {
                    result.WithNotFound("Tracking not found!");
                    return result;
                }

                await _trackingRepository.DeleteAsync(tracking);

                result.Message = "Tracking deleted successfully!";
                result.HasSuccess = true;
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}