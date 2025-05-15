using AcademiaLoja.Domain.Entities;

namespace AcademiaLoja.Application.Interfaces
{
    public interface IPaymentRepository : IBaseRepository<Payment>
    {
        Task<Payment> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Payment>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
        Task<Payment> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default);
        Task UpdatePaymentStatus(Guid paymentId, string status, string receiptUrl = null, string errorMessage = null, CancellationToken cancellationToken = default);
    }
}
