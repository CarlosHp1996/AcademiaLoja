using AcademiaLoja.Application.Models.Requests.Accessoriess;
using AcademiaLoja.Application.Models.Responses.Acessory;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Acessory
{
    public class CreateAcessoryCommand : IRequest<Result<AccessoryResponse>>
    {
        public CreateAccessoryRequest Request;
        public CreateAcessoryCommand(CreateAccessoryRequest request)
        {
            Request = request;
        }
    }
}
