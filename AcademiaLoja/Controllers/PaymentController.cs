using AcademiaLoja.Application.Commands.Payments;
using AcademiaLoja.Application.Models.Requests.Payments;
using AcademiaLoja.Application.Models.Responses.Payments;
using AcademiaLoja.Application.Queries.Payments;
using AcademiaLoja.Domain.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AcademiaLoja.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IMediator mediator, ILogger<PaymentController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [SwaggerOperation(
            Summary = "Create a payment intent for an order",
            Description = "Initializes a payment process with Stripe")]
        [SwaggerResponse(200, "Success", typeof(Result<CreatePaymentResponse>))]
        [HttpPost("create/{orderId}")]
        public async Task<IActionResult> CreatePayment(Guid orderId)
        {
            var command = new CreatePaymentCommand(orderId);
            var result = await _mediator.Send(command);

            if (result.HasSuccess)
                return Ok(result);

            return BadRequest(result.Errors);
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> HandleWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            var command = new ProcessWebhookCommand
            {
                JsonPayload = json,
                StripeSignature = Request.Headers["Stripe-Signature"]
            };

            var result = await _mediator.Send(command);

            if (result is null)
                return BadRequest();

            return Ok();
        }       

        [SwaggerOperation(
            Summary = "Confirm a payment",
            Description = "Confirms a payment with Stripe")]
        [SwaggerResponse(200, "Success", typeof(Result<ConfirmPaymentResponse>))]
        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmPayment([FromBody] ConfirmPaymentRequest request)
        {
            var command = new ConfirmPaymentCommand(request.PaymentIntentId, request.PaymentMethodId);
            var result = await _mediator.Send(command);

            if (result.HasSuccess)
                return Ok(result);

            return BadRequest(result.Errors);
        }

        [SwaggerOperation(
            Summary = "Verify payment status",
            Description = "Checks the current status of a payment")]
        [SwaggerResponse(200, "Success", typeof(Result<VerifyPaymentResponse>))]
        [HttpGet("verify/{paymentId}")]
        public async Task<IActionResult> VerifyPayment(Guid paymentId)
        {
            var query = new VerifyPaymentQuery(paymentId);
            var result = await _mediator.Send(query);

            if (result.HasSuccess)
                return Ok(result);

            return BadRequest(result.Errors);
        }

        [SwaggerOperation(
            Summary = "Refund a payment",
            Description = "Processes a refund request for a payment")]
        [SwaggerResponse(200, "Success", typeof(Result<RefundPaymentResponse>))]
        [HttpPost("refund/{paymentId}")]
        public async Task<IActionResult> RefundPayment(Guid paymentId, [FromBody] RefundPaymentRequest request)
        {
            var command = new RefundPaymentCommand(paymentId, request?.Amount ?? 0);
            var result = await _mediator.Send(command);

            if (result.HasSuccess)
                return Ok(result);

            return BadRequest(result.Errors);
        }
    }
}