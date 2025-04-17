using AcademiaLoja.Application.Models.Responses.Security;
using AcademiaLoja.Domain.Entities.Security;
using AcademiaLoja.Domain.Helpers;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AcademiaLoja.Application.Queries.Security.Handlers
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<GetUserByIdResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetUserByIdQueryHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<GetUserByIdResponse>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
        {
            var result = new Result<GetUserByIdResponse>();

            // Find the user by ID
            var user = await _userManager.FindByIdAsync(query.Id.ToString());
            if (user == null)
            {
                result.WithError("User not found.");
                return result;
            }

            // Prepare response
            var response = new GetUserByIdResponse
            {
                Id = user.Id,
                Name = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            result.Count = 1;
            result.Value = response;
            return result;
        }
    }
}
