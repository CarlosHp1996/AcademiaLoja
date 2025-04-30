using AcademiaLoja.Application.Models.Responses.Acessory;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Queries.Accessories
{
    public class GetAcessoryByIdQuery : IRequest<Result<AccessoryResponse>>
    {
        public Guid Id { get; set; }

        public GetAcessoryByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
