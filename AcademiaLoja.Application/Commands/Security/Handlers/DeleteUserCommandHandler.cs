using AcademiaLoja.Application.Models.Responses.Security;
using AcademiaLoja.Domain.Entities.Security;
using AcademiaLoja.Domain.Helpers;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AcademiaLoja.Application.Commands.Security.Handlers
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result<DeleteUserResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public DeleteUserCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<DeleteUserResponse>> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
        {
            var result = new Result<DeleteUserResponse>();

            // Find the user by ID
            var user = await _userManager.FindByIdAsync(command.Id.ToString());
            if (user == null)
            {
                result.WithError("User not found.");
                return result;
            }

            // Store user ID for response
            var userId = user.Id;

            // Delete the user
            var deleteResult = await _userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
            {
                result.WithError(deleteResult.Errors.First().Description);
                return result;
            }

            // Prepare response
            var response = new DeleteUserResponse
            {
                Id = userId,
                Message = "User deleted successfully"
            };

            result.Count = 1;
            result.Value = response;
            return result;
        }
    }
}
