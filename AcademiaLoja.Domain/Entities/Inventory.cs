﻿namespace AcademiaLoja.Domain.Entities
{
    public class Inventory
    {
        public Guid Id { get;  set; }
        public Guid ProductId { get;  set; }
        public int Quantity { get;  set; }
        public DateTime LastUpdated { get;  set; }

        // Navegação
        public virtual Product Product { get;  set; }
    }
}
