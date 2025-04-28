using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Objectives.Handlers
{
    public class DeleteObjectiveCommandHandler : IRequestHandler<DeleteObjectiveCommand, Result>
    {
        private readonly IObjectiveRepository _objectiveRepository;

        public DeleteObjectiveCommandHandler(IObjectiveRepository objectiveRepository)
        {
            _objectiveRepository = objectiveRepository;
        }

        public async Task<Result> Handle(DeleteObjectiveCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = new Result();
                var objective = await _objectiveRepository.GetById(request.Id);

                if (objective is null)
                {
                    result.WithNotFound("Objetivo não encontrado!");
                    return result;
                }

                await _objectiveRepository.DeleteAsync(objective);
                result.Message = "Objetivo excluído com sucesso!";
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao manipular o DeleteObjectiveCommand.", ex);
            }
        }
    }  
}
