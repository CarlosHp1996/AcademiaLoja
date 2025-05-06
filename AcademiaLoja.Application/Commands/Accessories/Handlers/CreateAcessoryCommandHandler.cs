using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Responses.Acessory;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Acessory.Handlers
{
    public class CreateAcessoryCommandHandler : IRequestHandler<CreateAcessoryCommand, Result<AccessoryResponse>>
    {
        private readonly IAccessoryRepository _accessoriesRepository;
        public CreateAcessoryCommandHandler(IAccessoryRepository accessoriesRepository)
        {
            _accessoriesRepository = accessoriesRepository;
        }
        public async Task<Result<AccessoryResponse>> Handle(CreateAcessoryCommand request, CancellationToken cancellationToken)
        {
            var result = new Result<AccessoryResponse>();

            try
            {
                // Delegar toda a lógica de criação para o repositório
                var response = await _accessoriesRepository.CreateAccessory(request.Request, cancellationToken);

                result.Value = response;
                result.Count = 1;
                result.HasSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.WithError($"Error creating accessory: {ex.Message}");
                return result;
            }
        }
    }
}
