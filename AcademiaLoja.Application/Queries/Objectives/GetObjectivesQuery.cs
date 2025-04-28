using AcademiaLoja.Application.Models.Filters;
using AcademiaLoja.Application.Models.Responses.Objectives;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Queries.Objectives
{
    public class GetObjectivesQuery : IRequest<Result<IEnumerable<ObjectiveResponse>>>
    {
        public GetObjectivesRequestFilter Filter { get; }
        public GetObjectivesQuery(GetObjectivesRequestFilter filter)
        {
            Filter = filter;
        }
    }
}
