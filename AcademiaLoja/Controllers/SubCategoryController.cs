using AcademiaLoja.Application.Commands.SubCategories;
using AcademiaLoja.Application.Models.Filters;
using AcademiaLoja.Application.Models.Requests.SubCategories;
using AcademiaLoja.Application.Models.Responses.SubCategories;
using AcademiaLoja.Application.Queries.SubCategories;
using AcademiaLoja.Domain.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AcademiaLoja.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubCategoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        public SubCategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [SwaggerOperation(
             Summary = "Create Subcategories",
             Description = "All fields are required.")]
        [SwaggerResponse(200, "Success", typeof(Result<SubCategoryResponse>))]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateSubCategoryRequest request)
        {
            var command = new CreateSubCategoryCommand(request);
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [SwaggerOperation(
         Summary = "Update Subcategories",
         Description = "SubCategory 'Id' is required.")]
        [SwaggerResponse(200, "Success", typeof(Result<SubCategoryResponse>))]
        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSubCategoryRequest request)
        {
            var command = new UpdateSubCategoryCommand(id, request);
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [SwaggerOperation(
             Summary = "List all Subcategories",
             Description = "List all Subcategories in a paginated manner.")]
        [SwaggerResponse(200, "Success", typeof(Result<SubCategoryResponse>))]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromQuery] GetSubCategoriesRequestFilter filter)
        {
            var command = new GetSubCategoriesQuery(filter);
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [SwaggerOperation(
            Summary = "List all Subcategories according to id",
            Description = "The Subcategory 'Id' is mandatory.")]
        [SwaggerResponse(200, "Success", typeof(Result<SubCategoryResponse>))]
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var command = new GetSubCategoryByIdQuery(id);
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [SwaggerOperation(
            Summary = "Delete Subcategory",
            Description = "SubCategory 'Id' is required.")]
        [SwaggerResponse(200, "Success", typeof(Result<SubCategoryResponse>))]
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteSubCategoryCommand(id);
            var response = await _mediator.Send(command);

            return Ok(response);
        }
    }
}
