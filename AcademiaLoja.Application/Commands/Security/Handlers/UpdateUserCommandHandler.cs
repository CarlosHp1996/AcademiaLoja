using AcademiaLoja.Application.Models.Responses.Security;
using AcademiaLoja.Domain.Entities.Security;
using AcademiaLoja.Domain.Helpers;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AcademiaLoja.Application.Commands.Security.Handlers
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<UpdateUserResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UpdateUserCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<UpdateUserResponse>> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            var result = new Result<UpdateUserResponse>();

            // Find the user by ID
            var user = await _userManager.FindByIdAsync(command.Request.Id.ToString());
            if (user == null)
            {
                result.WithError("User not found.");
                return result;
            }

            // Update user properties
            user.UserName = command.Request.Name;
            user.Email = command.Request.Email?.ToLower()?.Trim();
            user.PhoneNumber = command.Request.PhoneNumber;

            // Update user in database
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                result.WithError(updateResult.Errors.First().Description);
                return result;
            }

            // Update password if provided
            if (!string.IsNullOrEmpty(command.Request.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetPasswordResult = await _userManager.ResetPasswordAsync(user, token, command.Request.Password);

                if (!resetPasswordResult.Succeeded)
                {
                    result.WithError(resetPasswordResult.Errors.First().Description);
                    return result;
                }
            }

            // Prepare response
            var response = new UpdateUserResponse
            {
                Id = user.Id,
                Name = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Message = "User updated successfully"
            };

            result.Count = 1;
            result.Value = response;
            return result;
        }
    }
}
