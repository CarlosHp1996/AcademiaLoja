using AcademiaLoja.Application.Models.Requests.Objectives;
using AcademiaLoja.Application.Models.Responses.Objectives;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Objectives
{
    public class UpdateObjectiveCommand : IRequest<Result<ObjectiveResponse>>
    {
        public Guid Id { get; set; }
        public UpdateObjectiveRequest Request { get; set; }
        public UpdateObjectiveCommand(Guid id, UpdateObjectiveRequest request)
        {
            Id = id;
            Request = request;
        }
    }
}
