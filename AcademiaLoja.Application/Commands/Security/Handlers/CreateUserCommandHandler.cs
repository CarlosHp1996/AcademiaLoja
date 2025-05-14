using AcademiaLoja.Application.Models.Responses.Security;
using AcademiaLoja.Application.Services.Interfaces;
using AcademiaLoja.Domain.Entities.Security;
using AcademiaLoja.Domain.Helpers;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AcademiaLoja.Application.Commands.Security.Handlers
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<CreateUserResponse>>
    {
        private ApplicationUser _applicationUser;
        private UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public CreateUserCommandHandler(UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<Result<CreateUserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var result = new Result<CreateUserResponse>();
            var resultado = new IdentityResult();
            var validEmail = await _emailService.IsValidEmailAsync(request.Request.Email);

            // Validate if it is a valid email
            if (validEmail is false)
            {
                result.WithError("Invalid email format. Please provide a valid email address.");
                return result;
            }

            _applicationUser = new ApplicationUser();
            _applicationUser.Email = request.Request.Email != null ? request.Request.Email.ToLower()?.Trim() : null;
            _applicationUser.UserName = request.Request.Name;
            _applicationUser.PhoneNumber = request.Request.PhoneNumber;
            resultado = await _userManager.CreateAsync(_applicationUser, request.Request.Password);

            if (!resultado.Succeeded)
            {
                result.WithError(resultado.Errors.First().Description);
                return result;
            }

            var response = new CreateUserResponse
            {
                Id = _applicationUser.Id,
                Name = _applicationUser.UserName,
                Email = _applicationUser.Email,
                PhoneNumber = _applicationUser.PhoneNumber,
                Message = "User created successfully"
            };

            // Send email confirmation
            await _emailService.SendEmailConfirmationAsync(_applicationUser.Email);

            result.Count = 1;
            result.Value = response;
            return result;
        }
    }
}
