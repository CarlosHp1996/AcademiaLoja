using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Services.Interfaces;
using AcademiaLoja.Domain.Entities;
using AcademiaLoja.Domain.Helpers;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using Event = Stripe.Event;

namespace AcademiaLoja.Application.Commands.Payments.Handlers
{
    public class ProcessWebhookCommandHandler : IRequestHandler<ProcessWebhookCommand, Result>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProcessWebhookCommandHandler> _logger;

        public ProcessWebhookCommandHandler(
            IPaymentRepository paymentRepository,
            IOrderRepository orderRepository,
            IConfiguration configuration,
            ILogger<ProcessWebhookCommandHandler> logger)
        {
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<Result> Handle(ProcessWebhookCommand request, CancellationToken cancellationToken)
        {
            var result = new Result();

            try
            {
                var webhookSecret = Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_SECRET_ACADEMIA") ??
                         _configuration["Stripe:STRIPE_WEBHOOK_SECRET_ACADEMIA"];

                if (string.IsNullOrEmpty(webhookSecret))
                {
                    result.WithError("Stripe webhook secret is not configured");
                    return result;
                }

                var stripeEvent = EventUtility.ConstructEvent(
                    request.JsonPayload,
                    request.StripeSignature,
                    webhookSecret
                );

                // Processar diferentes tipos de eventos
                if (stripeEvent.Type == "payment_intent.created")
                {
                    await HandlePaymentIntentCreated(stripeEvent, cancellationToken);
                }
                else if (stripeEvent.Type == "payment_intent.succeeded")
                {
                    await HandlePaymentIntentSucceeded(stripeEvent, cancellationToken);
                }
                else if (stripeEvent.Type == "payment_intent.payment_failed")
                {
                    await HandlePaymentIntentFailed(stripeEvent, cancellationToken);
                }
                else if (stripeEvent.Type == "payment_intent.charge_refunded")
                {
                    await HandleChargeRefunded(stripeEvent, cancellationToken);
                }
                else
                {
                    _logger.LogInformation($"Unhandled event type: {stripeEvent.Type}");
                }

                result.HasSuccess = true;
                return result;
            }
            catch (StripeException e)
            {
                _logger.LogError(e, "Error processing Stripe webhook");
                result.WithError($"Stripe error: {e.Message}");
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error processing webhook");
                result.WithError($"Error: {e.Message}");
                return result;
            }
        }

        private async Task HandlePaymentIntentCreated(Event stripeEvent, CancellationToken cancellationToken)
        {
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            if (paymentIntent == null)
            {
                _logger.LogWarning($"Event {stripeEvent.Id}: PaymentIntent is null");
                return;
            }

            // Verificar se o orderId está nos valores dos metadados
            var orderId = paymentIntent.Metadata.Values.FirstOrDefault(v => Guid.TryParse(v, out _));
            if (string.IsNullOrEmpty(orderId))
            {
                _logger.LogWarning($"Event {stripeEvent.Id}: PaymentIntent {paymentIntent.Id} missing orderId in metadata values");
                return;
            }

            // Transformar string em Guid  
            if (!Guid.TryParse(orderId, out var orderGuid))
            {
                _logger.LogWarning($"Event {stripeEvent.Id}: Invalid orderId format in PaymentIntent {paymentIntent.Id}");
                return;
            }

            // Verifica se o pedido existe  
            var order = await _orderRepository.GetById(orderGuid, cancellationToken);
            if (order is null)
            {
                _logger.LogWarning($"Event {stripeEvent.Id}: Order {orderGuid} not found for PaymentIntent {paymentIntent.Id}");
                return;
            }

            // Verifica se o pagamento já existe
            var existingPayment = await _paymentRepository.GetByTransactionIdAsync(paymentIntent.Id, cancellationToken);
            if (existingPayment is null)
            {
                _logger.LogWarning($"Event {stripeEvent.Id}: PaymentIntent {paymentIntent.Id} already exists");
                return;
            }

            //Atualizar pagamento
            existingPayment.TransactionId = paymentIntent.Id;
            existingPayment.OrderId = orderGuid;
            existingPayment.Status = "created";
            existingPayment.PaymentMethod = "stripe";
            existingPayment.Amount = paymentIntent.Amount / 100m; // Convert from cents to decimal value
            existingPayment.Currency = paymentIntent.Currency;
            existingPayment.UpdatedAt = DateTime.UtcNow;       

            // Save the payment to the database
            await _paymentRepository.UpdateAsync(existingPayment);
            _logger.LogInformation($"PaymentIntent {paymentIntent.Id} created for Order {orderId}");
        }

        private async Task HandlePaymentIntentSucceeded(Event stripeEvent, CancellationToken cancellationToken)
        {
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            if (paymentIntent == null) return;

            // Encontrar o pagamento pelo TransactionId (PaymentIntent ID)  
            var payment = await _paymentRepository.GetByTransactionIdAsync(paymentIntent.Id, cancellationToken);
            if (payment == null) return;

            // Atualizar o status do pagamento  
            payment.Status = "succeeded";
            payment.ProcessedAt = DateTime.UtcNow;

            // Fix: Access the latest charge using the LatestCharge property instead of Charges  
            payment.ReceiptUrl = paymentIntent.LatestCharge?.ReceiptUrl;
            await _paymentRepository.UpdateAsync(payment);

            // Atualizar o status do pedido  
            var order = await _orderRepository.GetById(payment.OrderId, cancellationToken);
            if (order != null)
            {
                order.PaymentStatus = "Paid";
                order.Status = "Processing"; // Ou outro status apropriado  
                order.UpdatedAt = DateTime.UtcNow;
                await _orderRepository.UpdateAsync(order);
            }
        }

        private async Task HandlePaymentIntentFailed(Event stripeEvent, CancellationToken cancellationToken)
        {
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            if (paymentIntent == null) return;

            var payment = await _paymentRepository.GetByTransactionIdAsync(paymentIntent.Id, cancellationToken);
            if (payment == null) return;

            // Atualizar o status do pagamento
            payment.Status = "failed";
            payment.ProcessedAt = DateTime.UtcNow;
            payment.ErrorMessage = paymentIntent.LastPaymentError?.Message;
            await _paymentRepository.UpdateAsync(payment);

            // Atualizar o status do pedido
            var order = await _orderRepository.GetById(payment.OrderId, cancellationToken);
            if (order != null)
            {
                order.PaymentStatus = "Failed";
                order.UpdatedAt = DateTime.UtcNow;
                await _orderRepository.UpdateAsync(order);
            }
        }

        private async Task HandleChargeRefunded(Event stripeEvent, CancellationToken cancellationToken)
        {
            var charge = stripeEvent.Data.Object as Charge;
            if (charge == null || string.IsNullOrEmpty(charge.PaymentIntentId)) return;

            var payment = await _paymentRepository.GetByTransactionIdAsync(charge.PaymentIntentId, cancellationToken);
            if (payment == null) return;

            // Atualizar o status do pagamento
            payment.Status = "refunded";
            payment.ProcessedAt = DateTime.UtcNow;
            await _paymentRepository.UpdateAsync(payment);

            // Atualizar o status do pedido
            var order = await _orderRepository.GetById(payment.OrderId, cancellationToken);
            if (order != null)
            {
                order.PaymentStatus = "Refunded";
                order.UpdatedAt = DateTime.UtcNow;
                await _orderRepository.UpdateAsync(order);
            }
        }
    }
}