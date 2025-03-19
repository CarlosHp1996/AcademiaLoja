namespace AcademiaLoja.Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; private set; }
        public Guid OrderId { get; private set; }
        public string PaymentMethod { get; private set; }
        public string TransactionId { get; private set; }
        public decimal Amount { get; private set; }
        public string Status { get; private set; }
        public DateTime PaymentDate { get; private set; }

        // Navegação
        public virtual Order Order { get; private set; }
    }
}
