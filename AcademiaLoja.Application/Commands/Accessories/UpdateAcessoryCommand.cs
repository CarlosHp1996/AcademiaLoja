using AcademiaLoja.Application.Models.Requests.Acessory;
using AcademiaLoja.Application.Models.Responses.Acessory;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Acessory
{
    public class UpdateAcessoryCommand : IRequest<Result<AccessoryResponse>>
    {
        public Guid Id { get; set; }
        public UpdateAccessoryRequest Request;
        public UpdateAcessoryCommand(Guid id, UpdateAccessoryRequest request)
        {
            Id = id;
            Request = request;
        }
    }
}
