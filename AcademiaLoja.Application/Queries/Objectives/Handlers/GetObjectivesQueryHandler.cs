using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Responses.Objectives;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Queries.Objectives.Handlers
{
    public class GetObjectivesQueryHandler : IRequestHandler<GetObjectivesQuery, Result<IEnumerable<ObjectiveResponse>>>
    {
        private readonly IObjectiveRepository _objectiveRepository;

        public GetObjectivesQueryHandler(IObjectiveRepository objectiveRepository)
        {
            _objectiveRepository = objectiveRepository;
        }
        public async Task<Result<IEnumerable<ObjectiveResponse>>> Handle(GetObjectivesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = new Result<IEnumerable<ObjectiveResponse>>();
                //ALTERAR E CRIAR UM GET PERSONALIZADO IGUAL DO PRODUCT
                var objectives = await _objectiveRepository.GetAll(request.Filter.Take, request.Filter.Offset, request.Filter.SortingProp, request.Filter.Ascending);

                if (!objectives.Result(out var count).Any())
                {
                    result.WithError("Nenhum objetivo encontrado.");
                    return result;
                }

                var objectiveList = objectives.Result(out count);
                var response = new List<ObjectiveResponse>();

                foreach (var objective in objectiveList)
                {
                    var responseItem = new ObjectiveResponse
                    {
                        Id = objective.Id,
                        Name = objective.Name,
                        Description = objective.Description
                    };
                    response.Add(responseItem);
                }

                result.Value = response;
                result.Count = count;
                result.Message = "Objetivos listados com sucesso";
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao manipular o GetObjectivesQuery.", ex);
            }
        }
    }
}
