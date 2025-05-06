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
                accessories.Name = request.Request.Name ?? accessories.Name;
                accessories.Description = request.Request.Description ?? accessories.Description;
                accessories.Color = request.Request.Color ?? accessories.Color;
                accessories.Model = request.Request.Model ?? accessories.Model;
                accessories.Size = request.Request.Size ?? accessories.Size;

                _ = await _accessoriesRepository.UpdateAsync(accessories);

                var response = new AccessoryResponse
                {
                    Id = accessories.Id,
                    Name = accessories.Name,
                    Description = accessories.Description,
                    Color = accessories.Color,
                    Model = accessories.Model,
                    Size = accessories.Size
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
