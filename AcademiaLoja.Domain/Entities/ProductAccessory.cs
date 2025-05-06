namespace AcademiaLoja.Domain.Entities
{
    public class ProductAccessory
    {
        public Guid ProductId { get; set; }
        public Guid AccessoryId { get; set; }

        // Navegação
        public virtual Product Product { get; set; }
        public virtual Accessory Accessory { get; set; }
    }
}
