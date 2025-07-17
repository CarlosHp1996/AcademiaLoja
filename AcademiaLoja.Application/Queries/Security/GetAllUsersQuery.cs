using AcademiaLoja.Application.Models.Filters;
using AcademiaLoja.Application.Models.Responses.Security;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Queries.Security
{
    public class GetAllUsersQuery : IRequest<Result<GetAllUsersResponse>>
    {
        public GetUsersRequestFilter Filter { get; }

        public GetAllUsersQuery(GetUsersRequestFilter filter)
        {
            Filter = filter;
        }
    }
}
