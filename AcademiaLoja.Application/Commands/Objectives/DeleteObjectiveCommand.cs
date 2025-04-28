using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Objectives
{
    public class DeleteObjectiveCommand : IRequest<Result>
    {
        public Guid Id { get; }
        public DeleteObjectiveCommand(Guid id)
        {
            Id = id;
        }
    }
}
