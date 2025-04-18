using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Responses.Products;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Products.Handlers
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<CreateProductResponse>>
    {
        private readonly IProductRepository _productRepository;

        public CreateProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Result<CreateProductResponse>> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            var result = new Result<CreateProductResponse>();

            try
            {
                // Delegar toda a lógica de criação para o repositório
                var response = await _productRepository.CreateProduct(command.Request, cancellationToken);

                result.Value = response;
                result.Count = 1;
                result.HasSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.WithError($"Error creating product: {ex.Message}");
                return result;
            }
        }
    }
}
