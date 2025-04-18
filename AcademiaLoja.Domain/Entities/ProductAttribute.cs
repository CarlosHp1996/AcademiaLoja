namespace AcademiaLoja.Domain.Entities
{
    public class ProductAttribute
    {
        public Guid Id { get;  set; }
        public Guid ProductId { get;  set; }
        public string Key { get;  set; }
        public string Value { get;  set; }

        // Navegação
        public virtual Product Product { get;  set; }
    }
}
