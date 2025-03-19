namespace AcademiaLoja.Domain.Entities
{
    public class ProductAttribute
    {
        public Guid Id { get; private set; }
        public Guid ProductId { get; private set; }
        public string Key { get; private set; }
        public string Value { get; private set; }

        // Navegação
        public virtual Product Product { get; private set; }
    }
}
