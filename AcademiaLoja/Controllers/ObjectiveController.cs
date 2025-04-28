using AcademiaLoja.Application.Commands.Objectives;
using AcademiaLoja.Application.Models.Filters;
using AcademiaLoja.Application.Models.Requests.Objectives;
using AcademiaLoja.Application.Models.Responses.Objectives;
using AcademiaLoja.Application.Queries.Objectives;
using AcademiaLoja.Domain.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AcademiaLoja.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ObjectiveController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ObjectiveController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [SwaggerOperation(
             Summary = "Create Objectives",
             Description = "All fields are required.")]
        [SwaggerResponse(200, "Success", typeof(Result<ObjectiveResponse>))]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateObjectiveRequest request)
        {
            var command = new CreateObjectiveCommand(request);
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [SwaggerOperation(
         Summary = "Update Objectives",
         Description = "Objective 'Id' is required.")]
        [SwaggerResponse(200, "Success", typeof(Result<ObjectiveResponse>))]
        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateObjectiveRequest request)
        {
            var command = new UpdateObjectiveCommand(id, request);
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [SwaggerOperation(
             Summary = "List all Objectives",
             Description = "List all Objectives in a paginated manner.")]
        [SwaggerResponse(200, "Sucesso", typeof(Result<ObjectiveResponse>))]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromQuery] GetObjectivesRequestFilter filter)
        {
            var command = new GetObjectivesQuery(filter);
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [SwaggerOperation(
            Summary = "List all Objectives according to id",
            Description = "The Objective 'Id' is mandatory.")]
        [SwaggerResponse(200, "Success", typeof(Result<ObjectiveResponse>))]
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var command = new GetObjectiveByIdQuery(id);
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [SwaggerOperation(
            Summary = "Delete Objective",
            Description = "Objective 'Id' is required.")]
        [SwaggerResponse(200, "Success", typeof(Result<ObjectiveResponse>))]
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteObjectiveCommand(id);
            var response = await _mediator.Send(command);

            return Ok(response);
        }
    }
}
