using AcademiaLoja.Application.Models.Requests.Objectives;
using AcademiaLoja.Application.Models.Responses.Objectives;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Objectives
{
    public class CreateObjectiveCommand : IRequest<Result<ObjectiveResponse>>
    {
        public CreateObjectiveRequest Request;
        public CreateObjectiveCommand(CreateObjectiveRequest request)
        {
            Request = request;
        }

    }
}
