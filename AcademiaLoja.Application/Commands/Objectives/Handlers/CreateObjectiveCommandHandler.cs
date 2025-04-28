using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Responses.Objectives;
using AcademiaLoja.Domain.Entities;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Objectives.Handlers
{
    public class CreateObjectiveCommandHandler : IRequestHandler<CreateObjectiveCommand, Result<ObjectiveResponse>>
    {
        private readonly IObjectiveRepository _objectiveRepository;
        public CreateObjectiveCommandHandler(IObjectiveRepository objectiveRepository)
        {
            _objectiveRepository = objectiveRepository;
        }
        public async Task<Result<ObjectiveResponse>> Handle(CreateObjectiveCommand request, CancellationToken cancellationToken)
        {
            var result = new Result<ObjectiveResponse>();

            try
            {
                var objective = new Objective()
                {
                    Name = request.Request.Name,
                    Description = request.Request.Description
                };

                _ = await _objectiveRepository.AddAsync(objective);

                var response = new ObjectiveResponse
                {
                    Id = objective.Id,
                    Name = objective.Name,
                    Description = objective.Description
                };

                result.Value = response;
                result.Count = 1;
                result.HasSuccess = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao manipular o CreateObjectiveCommand.", ex);
            }

            return result;
        }
    }
}
