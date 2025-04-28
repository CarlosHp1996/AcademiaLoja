using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Responses.Objectives;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Queries.Objectives.Handlers
{
    public class GetObjectiveByIdQueryHandler : IRequestHandler<GetObjectiveByIdQuery, Result<ObjectiveResponse>>
    {
        private readonly IObjectiveRepository _objectiveRepository;
        public GetObjectiveByIdQueryHandler(IObjectiveRepository objectiveRepository)
        {
            _objectiveRepository = objectiveRepository;
        }
        public async Task<Result<ObjectiveResponse>> Handle(GetObjectiveByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = new Result<ObjectiveResponse>();
                //Criar getbyid com filtros e verificar os outros
                var objective = await _objectiveRepository.GetById(request.Id);

                if (objective == null)
                {
                    result.WithError("Nenhum objetivo encontrado.");
                    return result;
                }

                var response = new ObjectiveResponse
                {
                    Id = objective.Id,
                    Name = objective.Name,
                    Description = objective.Description
                };

                result.Value = response;
                result.Message = "Objetivo encontrado com sucesso";
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao manipular o GetObjectiveByIdQuery.", ex);
            }
        }
    }
}
