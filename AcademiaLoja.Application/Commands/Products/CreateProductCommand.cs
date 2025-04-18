using AcademiaLoja.Application.Models.Requests.Products;
using AcademiaLoja.Application.Models.Responses.Products;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Products
{
    public class CreateProductCommand : IRequest<Result<CreateProductResponse>>
    {
        public CreateProductRequest Request { get; }

        public CreateProductCommand(CreateProductRequest request)
        {
            Request = request;
        }
    }
}
