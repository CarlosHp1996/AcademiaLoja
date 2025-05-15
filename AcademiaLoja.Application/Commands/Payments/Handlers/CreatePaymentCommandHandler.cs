using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Responses.Payments;
using AcademiaLoja.Application.Services.Interfaces;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Commands.Payments.Handlers
{
    public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, Result<CreatePaymentResponse>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentService _paymentService;
        private readonly IPaymentRepository _paymentRepository;

        public CreatePaymentCommandHandler(
            IOrderRepository orderRepository,
            IPaymentService paymentService,
            IPaymentRepository paymentRepository)
        {
            _orderRepository = orderRepository;
            _paymentService = paymentService;
            _paymentRepository = paymentRepository;
        }

        public async Task<Result<CreatePaymentResponse>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            var result = new Result<CreatePaymentResponse>();

            try
            {
                // Get the order
                var order = await _orderRepository.GetById(request.OrderId, cancellationToken);
                if (order == null)
                {
                    result.WithError("Order not found");
                    return result;
                }

                // Check if order already has a pending payment
                var existingPayment = order.Payments
                    .FirstOrDefault(p => p.Status == "pending" && p.PaymentMethod == "stripe");

                if (existingPayment != null)
                {
                    // Return existing payment info
                    result.HasSuccess = true;
                    result.Value = new CreatePaymentResponse
                    {
                        PaymentId = existingPayment.Id,
                        OrderId = existingPayment.OrderId,
                        Amount = existingPayment.Amount,
                        ClientSecret = existingPayment.ClientSecret,
                        Status = existingPayment.Status
                    };
                    return result;
                }

                // Create a new payment intent with Stripe
                var payment = await _paymentService.CreatePaymentIntentAsync(order, cancellationToken);

                // Save payment to database
                await _paymentRepository.AddAsync(payment);

                // Update order payment status if needed
                if (order.PaymentStatus == "None" || string.IsNullOrEmpty(order.PaymentStatus))
                {
                    order.PaymentStatus = "Pending";
                    order.UpdatedAt = DateTime.UtcNow;
                    await _orderRepository.UpdateAsync(order);
                }

                // Return payment info
                var response = new CreatePaymentResponse
                {
                    PaymentId = payment.Id,
                    OrderId = payment.OrderId,
                    Amount = payment.Amount,
                    ClientSecret = payment.ClientSecret,
                    Status = payment.Status
                };

                result.Value = response;
                result.Count = 1;
                result.HasSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.WithError($"Error creating payment: {ex.Message}");
                return result;
            }
        }
    }
}
