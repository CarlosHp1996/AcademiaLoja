using AcademiaLoja.Application.Models.Filters;
using AcademiaLoja.Application.Models.Requests.Security;
using AcademiaLoja.Application.Models.Responses.Security;
using AcademiaLoja.Domain.Entities.Security;
using AcademiaLoja.Domain.Helpers;

namespace AcademiaLoja.Application.Interfaces
{
    public interface IUserRepository : IBaseRepository<ApplicationUser>
    {
        Task<CreateUserResponse> CreateUser(CreateUserRequest request, CancellationToken cancellationToken);
        Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request, CancellationToken cancellationToken);
        Task<bool> DeleteUser(Guid id, CancellationToken cancellationToken);
        Task<ApplicationUser> GetUserById(Guid id, CancellationToken cancellationToken);   
        Task<AsyncOutResult<IEnumerable<ApplicationUser>, int>> GetUsers(GetUsersRequestFilter filter, CancellationToken cancellationToken);
    }
}
