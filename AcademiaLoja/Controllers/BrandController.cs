using AcademiaLoja.Application.Commands.Brands;
using AcademiaLoja.Application.Models.Filters;
using AcademiaLoja.Application.Models.Requests.Brands;
using AcademiaLoja.Application.Models.Responses.Brands;
using AcademiaLoja.Application.Queries.Brands;
using AcademiaLoja.Domain.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AcademiaLoja.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandController : ControllerBase
    {
        private readonly IMediator _mediator;
        public BrandController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [SwaggerOperation(
             Summary = "Create brands",
             Description = "All fields are required.")]
        [SwaggerResponse(200, "Success", typeof(Result<BrandResponse>))]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateBrandRequest request)
        {
            var command = new CreateBrandCommand(request);
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [SwaggerOperation(
         Summary = "Update Brands",
         Description = "Category 'Id' is required.")]
        [SwaggerResponse(200, "Success", typeof(Result<BrandResponse>))]
        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBrandRequest request)
        {
            var command = new UpdateBrandCommand(id, request);
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [SwaggerOperation(
             Summary = "List all Brands",
             Description = "List all Brands in a paginated manner.")]
        [SwaggerResponse(200, "Sucesso", typeof(Result<BrandResponse>))]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromQuery] GetBrandsRequestFilter filter)
        {
            var command = new GetBrandsQuery(filter);
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [SwaggerOperation(
            Summary = "List all Brands according to id",
            Description = "The Brand 'Id' is mandatory.")]
        [SwaggerResponse(200, "Success", typeof(Result<BrandResponse>))]
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var command = new GetBrandByIdQuery(id);
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [SwaggerOperation(
            Summary = "Delete Brand",
            Description = "Brand 'Id' is required.")]
        [SwaggerResponse(200, "Success", typeof(Result<BrandResponse>))]
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteBrandCommand(id);
            var response = await _mediator.Send(command);

            return Ok(response);
        }
    }
}
