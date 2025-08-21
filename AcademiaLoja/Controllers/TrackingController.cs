using AcademiaLoja.Application.Commands.Trackings;
using AcademiaLoja.Application.Models.Filters;
using AcademiaLoja.Application.Models.Requests.Trackings;
using AcademiaLoja.Application.Models.Responses.Trackings;
using AcademiaLoja.Application.Queries.Trackings;
using AcademiaLoja.Domain.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AcademiaLoja.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TrackingController : ControllerBase
    {
        private readonly IMediator _mediator;
        public TrackingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Cria um novo evento de rastreamento para um pedido
        /// </summary>
        [SwaggerOperation(
             Summary = "Criar evento de rastreamento",
             Description = "Adiciona um novo evento ao histórico de rastreamento de um pedido.")]
        [SwaggerResponse(200, "Sucesso", typeof(Result<TrackingResponse>))]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTrackingRequest request)
        {
            var command = new CreateTrackingCommand(request);
            var response = await _mediator.Send(command);

            if (!response.HasSuccess)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Atualiza um evento de rastreamento existente
        /// </summary>
        [SwaggerOperation(
         Summary = "Atualizar evento de rastreamento",
         Description = "O 'Id' do evento de rastreamento é obrigatório.")]
        [SwaggerResponse(200, "Sucesso", typeof(Result<TrackingResponse>))]
        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin,Operator")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTrackingRequest request)
        {
            var command = new UpdateTrackingCommand(id, request);
            var response = await _mediator.Send(command);

            if (!response.HasSuccess)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Lista todos os eventos de rastreamento com paginação
        /// </summary>
        [SwaggerOperation(
             Summary = "Listar todos os eventos de rastreamento",
             Description = "Lista todos os eventos de rastreamento de forma paginada.")]
        [SwaggerResponse(200, "Sucesso", typeof(Result<TrackingResponse>))]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetTrackingsRequestFilter filter)
        {
            var query = new GetTrackingsQuery(filter);
            var response = await _mediator.Send(query);

            return Ok(response);
        }

        /// <summary>
        /// Obtém um evento de rastreamento específico pelo ID
        /// </summary>
        [SwaggerOperation(
            Summary = "Obter evento de rastreamento por ID",
            Description = "O 'Id' do evento de rastreamento é obrigatório.")]
        [SwaggerResponse(200, "Sucesso", typeof(Result<TrackingResponse>))]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetTrackingByIdQuery(id);
            var response = await _mediator.Send(query);

            if (!response.HasSuccess)
                return NotFound(response);

            return Ok(response);
        }        

        /// <summary>
        /// Exclui um evento de rastreamento
        /// </summary>
        [SwaggerOperation(
            Summary = "Excluir evento de rastreamento",
            Description = "O 'Id' do evento de rastreamento é obrigatório.")]
        [SwaggerResponse(200, "Sucesso", typeof(Result<TrackingResponse>))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteTrackingCommand(id);
            var result = await _mediator.Send(command);

            if (result.HasSuccess)
                return Ok(result);

            return BadRequest(result.Errors);
        }
    }
}