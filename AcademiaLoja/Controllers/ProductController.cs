using AcademiaLoja.Application.Commands.Products;
using AcademiaLoja.Application.Models.Filters;
using AcademiaLoja.Application.Models.Requests.Products;
using AcademiaLoja.Application.Models.Responses.Products;
using AcademiaLoja.Application.Queries.Products;
using AcademiaLoja.Domain.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AcademiaLoja.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [SwaggerOperation(
              Summary = "Create Product",
              Description = "Create a new product.")]
        [SwaggerResponse(200, "Success", typeof(Result<CreateProductResponse>))]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] CreateProductRequest request)
        {
            var command = new CreateProductCommand(request);
            var result = await _mediator.Send(command);

            if (result.HasSuccess)
                return Ok(result);

            return BadRequest(result.Errors);
        }

        [SwaggerOperation(
            Summary = "Get Product by ID",
            Description = "Retrieve a specific product by ID.")]
        [SwaggerResponse(200, "Success", typeof(Result<GetProductByIdResponse>))]
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetProductByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result.HasSuccess)
                return Ok(result);

            return NotFound(result.Errors);
        }

        [SwaggerOperation(
            Summary = "Get All Products",
            Description = "Retrieve all products with optional filtering and pagination.")]
        [SwaggerResponse(200, "Success", typeof(Result<GetAllProductsResponse>))]
        [HttpGet("get")]
        public async Task<IActionResult> GetAll([FromQuery] GetProductsRequestFilter filter)
        {
            var query = new GetAllProductsQuery(filter);
            var result = await _mediator.Send(query);

            if (result.HasSuccess)
                return Ok(result);

            return BadRequest(result.Errors);
        }

        [SwaggerOperation(
            Summary = "Update Product",
            Description = "Update an existing product.")]
        [SwaggerResponse(200, "Success", typeof(Result<UpdateProductResponse>))]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateProductRequest request)
        {
            var command = new UpdateProductCommand(id, request);
            var result = await _mediator.Send(command);

            if (result.HasSuccess)
                return Ok(result);

            return BadRequest(result.Errors);
        }

        [SwaggerOperation(
            Summary = "Delete Product",
            Description = "Delete a product by ID.")]
        [SwaggerResponse(200, "Success", typeof(Result))]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteProductCommand(id);
            var result = await _mediator.Send(command);

            if (result.HasSuccess)
                return Ok(result);

            return BadRequest(result.Errors);
        }
    }
}
