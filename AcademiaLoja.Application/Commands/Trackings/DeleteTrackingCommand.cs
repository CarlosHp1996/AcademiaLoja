using AcademiaLoja.Application.Models.Responses.Trackings;
using AcademiaLoja.Domain.Helpers;
using MediatR;
using System;

namespace AcademiaLoja.Application.Commands.Trackings
{
    public class DeleteTrackingCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public DeleteTrackingCommand(Guid id)
        {
            Id = id;
        }
    }
}
