using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Responses.Acessory;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Acessory.Handlers
{
    public class DeleteAcessoryCommandHandler : IRequestHandler<DeleteAcessoryCommand, Result<AccessoryResponse>>
    {
        private readonly IAccessoryRepository _accessoriesRepository;
        public DeleteAcessoryCommandHandler(IAccessoryRepository accessoriesRepository)
        {
            _accessoriesRepository = accessoriesRepository;
        }
        public async Task<Result<AccessoryResponse>> Handle(DeleteAcessoryCommand request, CancellationToken cancellationToken)
        {
            var result = new Result<AccessoryResponse>();

            try
            {
                var accessories = await _accessoriesRepository.GetById(request.Id);

                if (accessories == null)
                {
                    result.HasSuccess = false;
                    result.WithError("Accessório não encontrado.");
                    return result;
                }

                _ = await _accessoriesRepository.DeleteAsync(accessories);

                var response = new AccessoryResponse
                {
                    Id = accessories.Id,
                    Name = accessories.Name,
                    Description = accessories.Description,
                };

                result.Value = response;
                result.Count = 1;
                result.HasSuccess = true;
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }
    }
}
