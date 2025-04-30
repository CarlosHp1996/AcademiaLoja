using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Responses.Acessory;
using AcademiaLoja.Domain.Entities;
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
                var accessories = new Accessory()
                {
                    Name = request.Request.Name,
                    Description = request.Request.Description,
                };

                _ = await _accessoriesRepository.AddAsync(accessories);

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
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao manipular o CreateAcessoryCommand.", ex);
            }

            return result;
        }
    }
}
