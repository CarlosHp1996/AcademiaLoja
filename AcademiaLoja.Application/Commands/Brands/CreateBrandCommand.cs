using AcademiaLoja.Application.Models.Requests.Brands;
using AcademiaLoja.Application.Models.Responses.Brands;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Brands
{
    public class CreateBrandCommand : IRequest<Result<BrandResponse>>
    {
        public CreateBrandRequest Request;
        public CreateBrandCommand(CreateBrandRequest request)
        {
            Request = request;
        }
    }
}
