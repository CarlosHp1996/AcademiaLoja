using AcademiaLoja.Application.Models.Requests.Brands;
using AcademiaLoja.Application.Models.Responses.Brands;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Brands
{
    public class UpdateBrandCommand : IRequest<Result<BrandResponse>>
    {
        public Guid Id { get; set; }
        public UpdateBrandRequest Request { get; set; }
        public UpdateBrandCommand(Guid id, UpdateBrandRequest request)
        {
            Id = id;
            Request = request;
        }
    }
}
