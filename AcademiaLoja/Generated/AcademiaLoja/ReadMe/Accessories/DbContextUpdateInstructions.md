# Instruções para atualizar o AppDbContext

## 1. Adicione a propriedade DbSet no AppDbContext.cs

```csharp
public DbSet<Accessories> Accessoriess { get; set; }
```

## 2. Adicione a configuração no método OnModelCreating

```csharp
// Configuração de Accessories
modelBuilder.Entity<Accessories>(entity =>
{
    entity.ToTable("Accessoriess");
    entity.HasKey(c => c.Id);

    entity.Property(c => c.Name)
        .IsRequired()
        .HasMaxLength(0)
        ;

    entity.Property(c => c.Description)
        .IsRequired()
        .HasMaxLength(0)
        ;

    // Relacionamentos
    entity.HasMany(c => c.ProductAccessoriess)
        .WithOne(pc => pc.Accessories)
        .HasForeignKey(pc => pc.AccessoriesId);
});
```

