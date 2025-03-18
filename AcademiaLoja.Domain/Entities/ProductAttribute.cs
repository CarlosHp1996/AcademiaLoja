namespace AcademiaLoja.Domain.Entities
{
    public class ProductAttribute
    {
        public int Id { get; private set; }
        public int ProductId { get; private set; }
        public string Key { get; private set; }
        public string Value { get; private set; }

        // Navegação
        public virtual Product Product { get; private set; }
    }
}
