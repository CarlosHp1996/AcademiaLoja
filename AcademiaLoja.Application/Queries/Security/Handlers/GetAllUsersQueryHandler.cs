using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Dtos;
using AcademiaLoja.Application.Models.Filters;
using AcademiaLoja.Application.Models.Responses.Security;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Queries.Security.Handlers
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<GetAllUsersResponse>>
    {
        private readonly IUserRepository _repository;

        public GetAllUsersQueryHandler(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<GetAllUsersResponse>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
        {
            var result = new Result<GetAllUsersResponse>();

            try
            {
                var filter = query.Filter ?? new GetUsersRequestFilter();
                var usersResult = await _repository.GetUsers(filter, cancellationToken);

                // Extract the IEnumerable<ApplicationUser> and the count from AsyncOutResult  
                var users = usersResult.Result(out int totalCount);

                if (users == null || !users.Any())
                {
                    result.WithError("No users found.");
                    return result;
                }

                // Map users to response  
                var response = new GetAllUsersResponse
                {
                    Users = users.Select(user => new UserDto
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        Cpf = user.Cpf,
                        Gender = user.Gender,
                        PhoneNumber = user.PhoneNumber,
                        Addresses = user.Addresses?.Select(address => new AddressDto
                        {
                            Id = address.Id,
                            Street = address.Street,
                            City = address.City,
                            State = address.State,
                            ZipCode = address.ZipCode,
                            Neighborhood = address.Neighborhood,
                            Number = address.Number,
                            Complement = address.Complement
                        }).ToList() ?? new List<AddressDto>()
                    }),
                    TotalCount = totalCount,
                    PageCount = (int)Math.Ceiling((decimal)((double)totalCount / filter.PageSize)),
                    CurrentPage = filter.Page,
                    PageSize = filter.PageSize
                };

                result.Value = response;
                result.HasSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.WithError($"Error retrieving users: {ex.Message}");
                return result;
            }
        }
    }
}
