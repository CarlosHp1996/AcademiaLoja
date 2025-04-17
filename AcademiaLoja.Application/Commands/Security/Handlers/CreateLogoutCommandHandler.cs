using AcademiaLoja.Application.Models.Responses.Security;
using AcademiaLoja.Domain.Helpers;
using AcademiaLoja.Domain.Security;
using MediatR;

namespace AcademiaLoja.Application.Commands.Security.Handlers
{
    public class CreateLogoutCommandHandler : IRequestHandler<CreateLogoutCommand, Result<CreateLogoutResponse>>
    {
        private readonly AccessManager _accessManager;

        public CreateLogoutCommandHandler(AccessManager accessManager)
        {
            _accessManager = accessManager;
        }

        public async Task<Result<CreateLogoutResponse>> Handle(CreateLogoutCommand request, CancellationToken cancellationToken)
        {
            var result = new Result<CreateLogoutResponse>();

            await _accessManager.InvalidateToken(request.Request.Token);

            var response = new CreateLogoutResponse
            {
                Message = "Logout realizado com sucesso."
            };

            result.Value = response;
            result.Count = 1;
            return result;
        }
    }
}
