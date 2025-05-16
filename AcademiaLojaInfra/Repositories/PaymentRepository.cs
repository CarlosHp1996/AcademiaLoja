using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Domain.Entities;
using AcademiaLoja.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace AcademiaLoja.Infra.Repositories
{
    public class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
    {
        private readonly AppDbContext _context;

        public PaymentRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Payment> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Payments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<Payment> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            return await _context.Payments
                .Where(p => p.OrderId == orderId)
                .OrderByDescending(p => p.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Payment> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default)
        {
            return await _context.Payments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.TransactionId == transactionId, cancellationToken);
        }

        public async Task UpdatePaymentStatus(Guid paymentId, string status, string receiptUrl = null, string errorMessage = null, CancellationToken cancellationToken = default)
        {
            var payment = await _context.Payments.FindAsync(new object[] { paymentId }, cancellationToken);

            if (payment != null)
            {
                payment.Status = status;
                payment.ProcessedAt = DateTime.UtcNow;

                if (receiptUrl != null)
                    payment.ReceiptUrl = receiptUrl;

                if (errorMessage != null)
                    payment.ErrorMessage = errorMessage;

                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
