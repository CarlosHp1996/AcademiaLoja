using AcademiaLoja.Application.Commands.Categories;
using AcademiaLoja.Application.Models.Filters;
using AcademiaLoja.Application.Models.Requests.Categories;
using AcademiaLoja.Application.Models.Responses.Categories;
using AcademiaLoja.Application.Queries.Categories;
using AcademiaLoja.Domain.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AcademiaLoja.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [SwaggerOperation(
             Summary = "Create categories",
             Description = "All fields are required.")]
        [SwaggerResponse(200, "Success", typeof(Result<CategoryResponse>))]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
        {
            var command = new CreateCategoryCommand(request);
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [SwaggerOperation(
         Summary = "Update Categories",
         Description = "Category 'Id' is required.")]
        [SwaggerResponse(200, "Success", typeof(Result<CategoryResponse>))]
        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequest request)
        {
            var command = new UpdateCategoryCommand(id, request);
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [SwaggerOperation(
             Summary = "List all categories",
             Description = "List all categories in a paginated manner.")]
        [SwaggerResponse(200, "Sucesso", typeof(Result<CategoryResponse>))]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromQuery] GetCategoriesRequestFilter filter)
        {
            var command = new GetCategoriesQuery(filter);
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [SwaggerOperation(
            Summary = "List all categories according to id",
            Description = "The category 'Id' is mandatory.")]
        [SwaggerResponse(200, "Success", typeof(Result<CategoryResponse>))]
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var command = new GetCategoryByIdQuery(id);
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [SwaggerOperation(
            Summary = "Delete category",
            Description = "Category 'Id' is required.")]
        [SwaggerResponse(200, "Success", typeof(Result<CategoryResponse>))]
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteCategoryCommand(id);
            var response = await _mediator.Send(command);

            return Ok(response);
        }
    }
}
