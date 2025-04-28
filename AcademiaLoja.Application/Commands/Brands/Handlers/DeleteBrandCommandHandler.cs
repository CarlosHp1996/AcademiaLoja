using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Brands.Handlers
{
    public class DeleteBrandCommandHandler : IRequestHandler<DeleteBrandCommand, Result>
    {
        private readonly IBrandRepository _brandRepository;

        public DeleteBrandCommandHandler(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public async Task<Result> Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = new Result();
                var brand = await _brandRepository.GetById(request.Id);

                if (brand is null)
                {
                    result.WithNotFound("Marca não encontrada!");
                    return result;
                }

                await _brandRepository.DeleteAsync(brand);
                result.Message = "Marca excluída com sucesso!";
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao manipular o DeleteBrandCommand.", ex);
            }
        }
    }
}
