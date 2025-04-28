namespace AcademiaLoja.Domain.Entities
{
    public class Objective
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // Navegação
        public virtual ICollection<ProductObjective> ProductObjectives { get; private set; } = new List<ProductObjective>();
    }
}
