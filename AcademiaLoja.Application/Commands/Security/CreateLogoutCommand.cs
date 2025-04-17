using AcademiaLoja.Application.Models.Requests.Security;
using AcademiaLoja.Application.Models.Responses.Security;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Security
{
    public class CreateLogoutCommand : IRequest<Result<CreateLogoutResponse>>
    {
        public CreateLogoutRequest Request;

        public CreateLogoutCommand(CreateLogoutRequest request)
        {
            Request = request;
        }
    }
}
