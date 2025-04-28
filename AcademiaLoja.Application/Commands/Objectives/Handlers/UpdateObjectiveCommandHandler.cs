using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Responses.Objectives;
using AcademiaLoja.Domain.Helpers;
using MediatR;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AcademiaLoja.Application.Commands.Objectives.Handlers
{
    public class UpdateObjectiveCommandHandler : IRequestHandler<UpdateObjectiveCommand, Result<ObjectiveResponse>>
    {
        private readonly IObjectiveRepository _objectiveRepository;
        public UpdateObjectiveCommandHandler(IObjectiveRepository objectiveRepository)
        {
            _objectiveRepository = objectiveRepository;
        }
        public async Task<Result<ObjectiveResponse>> Handle(UpdateObjectiveCommand request, CancellationToken cancellationToken)
        {
            var result = new Result<ObjectiveResponse>();

            try
            {                
                var objective = await _objectiveRepository.GetById(request.Id);

                if (objective == null)
                {
                    result.WithError("Nenhum objetivo encontrado.");
                    return result;
                }

                objective.Name = request.Request.Name;
                objective.Description = request.Request.Description;
                var update = await _objectiveRepository.UpdateAsync(objective);

                if (!update)
                {
                    result.WithError("Erro ao atualizar o objetivo.");
                    return result;
                }

                var response = new ObjectiveResponse
                {
                    Id = objective.Id,
                    Name = objective.Name,
                    Description = objective.Description
                };

                result.Value = response;
                result.Count = 1;
                result.HasSuccess = true;
                result.Message = "Objetivo atualizado com sucesso";
                
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao manipular o UpdateObjectiveCommand.", ex);
            }

            return result;
        }
    }
}
