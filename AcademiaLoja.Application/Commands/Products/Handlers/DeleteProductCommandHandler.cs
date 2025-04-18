using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Products.Handlers
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result>
    {
        private readonly IProductRepository _productRepository;

        public DeleteProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = new Result();
                var product = await _productRepository.GetById(request.Id);

                if (product is null)
                {
                    result.WithNotFound("Product not found!");
                    return result;
                }

                await _productRepository.DeleteAsync(product);

                result.Message = "Product deleted successfully!";
                result.HasSuccess = true;
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
