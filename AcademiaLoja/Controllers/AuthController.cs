using AcademiaLoja.Application.Commands.Security;
using AcademiaLoja.Application.Models.Requests.Security;
using AcademiaLoja.Application.Models.Responses.Security;
using AcademiaLoja.Domain.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AcademiaLoja.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [SwaggerOperation(
              Summary = "Create User",
              Description = "Create a new user.")]
        [SwaggerResponse(200, "Success", typeof(Result<CreateUserResponse>))]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
        {
            var command = new CreateUserCommand(request);
            var result = await _mediator.Send(command);

            if (result.HasSuccess)
                return Ok(result);

            return BadRequest(result.Errors);
        }

        [SwaggerOperation(
           Summary = "User Login",
           Description = "User login and authentication token generation.")]
        [SwaggerResponse(200, "Sucesso", typeof(Result<CreateLoginResponse>))]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] CreateLoginRequest request)
        {
            var command = new CreateLoginCommand(request);
            var result = await _mediator.Send(command);

            if (result.HasSuccess)
                return Ok(result);

            return BadRequest(result.Errors);
        }

        [SwaggerOperation(
           Summary = "User Logout",
           Description = "Logs out the user and invalidates the JWT token.")]
        [SwaggerResponse(200, "Sucesso", typeof(Result<CreateLogoutResponse>))]
        [HttpPost("logout")]
        //[Authorize(Roles = "Usuario,Admin")]
        public async Task<IActionResult> Logout([FromBody] CreateLogoutRequest request)
        {
            var command = new CreateLogoutCommand(request);
            var result = await _mediator.Send(command);

            if (result.HasSuccess)
                return Ok(result);

            return BadRequest(result.Errors);
        }

        [SwaggerOperation(
           Summary = "Update User",
           Description = "Update existing user information.")]
        [SwaggerResponse(200, "Success", typeof(Result<UpdateUserResponse>))]
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UpdateUserRequest request)
        {
            var command = new UpdateUserCommand(request);
            var result = await _mediator.Send(command);

            if (result.HasSuccess)
                return Ok(result);

            return BadRequest(result.Errors);
        }

        [SwaggerOperation(
           Summary = "Get User by ID",
           Description = "Retrieve a specific user by ID.")]
        [SwaggerResponse(200, "Success", typeof(Result<GetUserByIdResponse>))]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetUserByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result.HasSuccess)
                return Ok(result);

            return NotFound(result.Errors);
        }

        [SwaggerOperation(
           Summary = "Get All Users",
           Description = "Retrieve all users with optional filtering and pagination.")]
        [SwaggerResponse(200, "Success", typeof(Result<GetAllUsersResponse>))]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllUsersRequest request)
        {
            var query = new GetAllUsersQuery(request);
            var result = await _mediator.Send(query);

            if (result.HasSuccess)
                return Ok(result);

            return BadRequest(result.Errors);
        }

        [SwaggerOperation(
           Summary = "Delete User",
           Description = "Delete a user by ID.")]
        [SwaggerResponse(200, "Success", typeof(Result<DeleteUserResponse>))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteUserCommand(id);
            var result = await _mediator.Send(command);

            if (result.HasSuccess)
                return Ok(result);

            return BadRequest(result.Errors);
        }
    }
}
