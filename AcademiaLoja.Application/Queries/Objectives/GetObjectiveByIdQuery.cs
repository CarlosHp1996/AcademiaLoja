using AcademiaLoja.Application.Models.Responses.Objectives;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Queries.Objectives
{
    public class GetObjectiveByIdQuery : IRequest<Result<ObjectiveResponse>>
    {
        public Guid Id { get; }
        public GetObjectiveByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
