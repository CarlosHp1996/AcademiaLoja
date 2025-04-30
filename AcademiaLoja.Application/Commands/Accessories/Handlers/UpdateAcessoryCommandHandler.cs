using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Responses.Acessory;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Acessory.Handlers
{
    public class UpdateAcessoryCommandHandler : IRequestHandler<UpdateAcessoryCommand, Result<AccessoryResponse>>
    {
        private readonly IAccessoryRepository _accessoriesRepository;
        public UpdateAcessoryCommandHandler(IAccessoryRepository accessoriesRepository)
        {
            _accessoriesRepository = accessoriesRepository;
        }
        public async Task<Result<AccessoryResponse>> Handle(UpdateAcessoryCommand request, CancellationToken cancellationToken)
        {
            var result = new Result<AccessoryResponse>();

            try
            {
                var accessories = await _accessoriesRepository.GetById(request.Id);

                if (accessories == null)
                {
                    result.HasSuccess = false;
                    result.Message = "Accessories não encontrado.";
                    return result;
                }

                accessories.Id = request.Id;
                accessories.Name = request.Request.Name;
                accessories.Description = request.Request.Description;

                _ = await _accessoriesRepository.UpdateAsync(accessories);

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
