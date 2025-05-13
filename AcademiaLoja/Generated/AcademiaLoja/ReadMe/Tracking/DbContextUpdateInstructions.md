# Instruções para atualizar o AppDbContext

## 1. Adicione a propriedade DbSet no AppDbContext.cs

```csharp
public DbSet<Tracking> Trackings { get; set; }
```

## 2. Adicione a configuração no método OnModelCreating

```csharp
// Configuração de Tracking
modelBuilder.Entity<Tracking>(entity =>
{
    entity.ToTable("Trackings");
    entity.HasKey(c => c.Id);

    entity.Property(c => c.OrderId)
        .IsRequired()
        .HasMaxLength(0)
        ;

    entity.Property(c => c.Status)
        .IsRequired()
        .HasMaxLength(50)
        ;

    entity.Property(c => c.Description)
        .HasMaxLength(500)
        ;

    entity.Property(c => c.Location)
        .HasMaxLength(100)
        ;

    // Relacionamentos
    entity.HasMany(c => c.ProductTrackings)
        .WithOne(pc => pc.Tracking)
        .HasForeignKey(pc => pc.TrackingId);
});
```

