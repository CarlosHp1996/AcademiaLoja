namespace AcademiaLoja.Domain.Entities
{
    public class ProductObjective
    {
        public Guid ProductId { get; set; }
        public Guid ObjectiveId { get; set; }

        // Navegação
        public virtual Product Product { get; set; }
        public virtual Objective Objective { get; set; }
    }
}
