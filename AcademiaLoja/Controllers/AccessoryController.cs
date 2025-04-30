using AcademiaLoja.Application.Commands.Acessory;
using AcademiaLoja.Application.Models.Filters;
using AcademiaLoja.Application.Models.Requests.Accessoriess;
using AcademiaLoja.Application.Models.Requests.Acessory;
using AcademiaLoja.Application.Models.Responses.Acessory;
using AcademiaLoja.Application.Queries.Accessoriess;
using AcademiaLoja.Domain.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AcademiaLoja.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccessoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AccessoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [SwaggerOperation(
             Summary = "Create Acessory",
             Description = "All fields are required.")]
        [SwaggerResponse(200, "Success", typeof(Result<AccessoryResponse>))]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateAccessoryRequest request)
        {
            var command = new CreateAcessoryCommand(request);
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [SwaggerOperation(
         Summary = "Update Acessory",
         Description = "Accessories 'Id' is required.")]
        [SwaggerResponse(200, "Success", typeof(Result<AccessoryResponse>))]
        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAccessoryRequest request)
        {
            var command = new UpdateAcessoryCommand(id, request);
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [SwaggerOperation(
             Summary = "List all Accessoriess",
             Description = "List all Accessoriess in a paginated manner.")]
        [SwaggerResponse(200, "Sucesso", typeof(Result<AccessoryResponse>))]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromQuery] GetAccessoriessRequestFilter filter)
        {
            var command = new GetAccessoriesQuery(filter);
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [SwaggerOperation(
            Summary = "List Acessory according to id",
            Description = "The Acessory 'Id' is mandatory.")]
        [SwaggerResponse(200, "Success", typeof(Result<AccessoryResponse>))]
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var command = new GetAcessoryByIdQuery(id);
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [SwaggerOperation(
            Summary = "Delete Acessory",
            Description = "Acessory 'Id' is required.")]
        [SwaggerResponse(200, "Success", typeof(Result<AccessoryResponse>))]
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteAcessoryCommand(id);
            var response = await _mediator.Send(command);

            return Ok(response);
        }
    }
}
