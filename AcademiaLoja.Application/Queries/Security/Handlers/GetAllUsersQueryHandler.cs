using AcademiaLoja.Application.Models.Responses.Security;
using AcademiaLoja.Domain.Entities.Security;
using AcademiaLoja.Domain.Helpers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AcademiaLoja.Application.Queries.Security.Handlers
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<GetAllUsersResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetAllUsersQueryHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<GetAllUsersResponse>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
        {
            var result = new Result<GetAllUsersResponse>();

            try
            {
                // Get users queryable
                var usersQuery = _userManager.Users.AsQueryable();

                // Apply search filter if provided
                if (!string.IsNullOrEmpty(query.Request.SearchTerm))
                {
                    string searchTerm = query.Request.SearchTerm.ToLower();
                    usersQuery = usersQuery.Where(u =>
                        u.UserName.ToLower().Contains(searchTerm) ||
                        u.Email.ToLower().Contains(searchTerm) ||
                        (u.PhoneNumber != null && u.PhoneNumber.Contains(searchTerm))
                    );
                }

                // Apply sorting
                if (!string.IsNullOrEmpty(query.Request.SortBy))
                {
                    // Default to ascending order if not specified
                    bool ascending = query.Request.SortAscending ?? true;

                    switch (query.Request.SortBy.ToLower())
                    {
                        case "name":
                            usersQuery = ascending ?
                                usersQuery.OrderBy(u => u.UserName) :
                                usersQuery.OrderByDescending(u => u.UserName);
                            break;
                        case "email":
                            usersQuery = ascending ?
                                usersQuery.OrderBy(u => u.Email) :
                                usersQuery.OrderByDescending(u => u.Email);
                            break;
                        case "phonenumber":
                            usersQuery = ascending ?
                                usersQuery.OrderBy(u => u.PhoneNumber) :
                                usersQuery.OrderByDescending(u => u.PhoneNumber);
                            break;
                        default:
                            usersQuery = ascending ?
                                usersQuery.OrderBy(u => u.Id) :
                                usersQuery.OrderByDescending(u => u.Id);
                            break;
                    }
                }
                else
                {
                    // Default sort by ID
                    usersQuery = usersQuery.OrderBy(u => u.Id);
                }

                // Get total count
                int totalCount = await usersQuery.CountAsync(cancellationToken);

                // Apply pagination
                int pageSize = query.Request.PageSize ?? 10;  // Default page size
                int page = query.Request.Page ?? 1;  // Default to first page
                int skip = (page - 1) * pageSize;

                var users = await usersQuery
                    .Skip(skip)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                // Calculate total pages
                int pageCount = (int)Math.Ceiling(totalCount / (double)pageSize);

                // Create response
                var response = new GetAllUsersResponse
                {
                    Users = users.Select(u => new UserDto
                    {
                        Id = u.Id,
                        Name = u.UserName,
                        Email = u.Email,
                        PhoneNumber = u.PhoneNumber
                    }),
                    TotalCount = totalCount,
                    PageCount = pageCount,
                    CurrentPage = page,
                    PageSize = pageSize
                };

                result.Value = response;
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
