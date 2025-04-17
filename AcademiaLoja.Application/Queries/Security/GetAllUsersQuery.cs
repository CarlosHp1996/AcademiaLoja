using AcademiaLoja.Application.Models.Requests.Security;
using AcademiaLoja.Application.Models.Responses.Security;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Queries.Security
{
    public class GetAllUsersQuery : IRequest<Result<GetAllUsersResponse>>
    {
        public GetAllUsersRequest Request { get; }

        public GetAllUsersQuery(GetAllUsersRequest request)
        {
            Request = request;
        }
    }
}
