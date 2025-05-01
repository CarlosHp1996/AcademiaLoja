using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Responses.Products;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Products.Handlers
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<UpdateProductResponse>>
    {
        private readonly IProductRepository _productRepository;

        public UpdateProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Result<UpdateProductResponse>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var result = new Result<UpdateProductResponse>();

            try
            {
                // Verificar se o produto existe
                var product = await _productRepository.GetById(request.Id);

                if (product == null)
                {
                    result.WithError("Product not found.");
                    return result;
                }

                // Delegar toda a lógica de atualização para o repositório
                var response = await _productRepository.UpdateProduct(product, request.Request, cancellationToken);

                result.Value = response;
                result.HasSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.WithError($"Error updating product: {ex.Message}");
                return result;
            }
        }
    }
}
