using AcademiaLoja.Application.Models.Responses.Security;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Queries.Security
{
    public class GetUserByIdQuery : IRequest<Result<GetUserByIdResponse>>
    {
        public Guid Id { get; }

        public GetUserByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
