using AcademiaLoja.Application.Models.Requests.Products;
using AcademiaLoja.Application.Models.Responses.Products;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Products
{
    public class UpdateProductCommand : IRequest<Result<UpdateProductResponse>>
    {
        public Guid Id;
        public UpdateProductRequest Request { get; }

        public UpdateProductCommand(Guid id, UpdateProductRequest request)
        {
            Id = id;
            Request = request;
        }
    }
}
