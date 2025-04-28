using AcademiaLoja.Domain.Entities;
using AcademiaLoja.Domain.Entities.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AcademiaLoja.Infra.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ProductAttribute> ProductAttributes { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<CategorySubCategory> CategorySubCategories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<ProductBrand> ProductBrands { get; set; }
        public DbSet<Objective> Objectives { get; set; }
        public DbSet<ProductObjective> ProductObjectives { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração de Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(p => p.Description)
                    .IsRequired();

                entity.Property(p => p.Price)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                entity.Property(p => p.StockQuantity)
                    .IsRequired();

                entity.Property(p => p.ImageUrl)
                    .HasMaxLength(500);

                entity.Property(p => p.IsActive)
                    .IsRequired();

                entity.Property(p => p.CreatedAt)
                    .IsRequired();

                entity.Property(p => p.UpdatedAt)
                    .IsRequired();

                // Relacionamentos
                entity.HasMany(p => p.ProductCategories)
                    .WithOne(pc => pc.Product)
                    .HasForeignKey(pc => pc.ProductId);

                entity.HasMany(p => p.OrderItems)
                    .WithOne(oi => oi.Product)
                    .HasForeignKey(oi => oi.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(p => p.Attributes)
                    .WithOne(pa => pa.Product)
                    .HasForeignKey(pa => pa.ProductId);

                entity.HasOne(p => p.Inventory)
                    .WithOne(i => i.Product)
                    .HasForeignKey<Inventory>(i => i.ProductId);
            });

            // Configuração de Category
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(c => c.Description)
                    .HasMaxLength(500);

                // Relacionamentos
                entity.HasMany(c => c.ProductCategories)
                    .WithOne(pc => pc.Category)
                    .HasForeignKey(pc => pc.CategoryId);

                // Relacionamentos
                entity.HasMany(p => p.CategorySubCategories)
                    .WithOne(pc => pc.Category)
                    .HasForeignKey(pc => pc.CategoryId);
            });

            // Configuração de ProductCategory
            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.ToTable("ProductCategories");
                entity.HasKey(pc => new { pc.ProductId, pc.CategoryId });
            });

            // Configuração de Order
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders");
                entity.HasKey(o => o.Id);

                entity.Property(o => o.TotalAmount)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                entity.Property(o => o.Status)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(o => o.PaymentStatus)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(o => o.TrackingNumber)
                    .HasMaxLength(50);

                entity.Property(o => o.ShippingAddress)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(o => o.OrderDate)
                    .IsRequired();

                entity.Property(o => o.UpdatedAt)
                    .IsRequired();

                // Relacionamentos
                entity.HasOne(o => o.User)
                    .WithMany()
                    .HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(o => o.OrderItems)
                    .WithOne(oi => oi.Order)
                    .HasForeignKey(oi => oi.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(o => o.Payments)
                    .WithOne(p => p.Order)
                    .HasForeignKey(p => p.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuração de OrderItem
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("OrderItems");
                entity.HasKey(oi => oi.Id);

                entity.Property(oi => oi.Quantity)
                    .IsRequired();

                entity.Property(oi => oi.UnitPrice)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");
            });

            // Configuração de ProductAttribute
            modelBuilder.Entity<ProductAttribute>(entity =>
            {
                entity.ToTable("ProductAttributes");
                entity.HasKey(pa => pa.Id);

                entity.Property(pa => pa.Key)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(pa => pa.Value)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            // Configuração de Inventory
            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.ToTable("Inventory");
                entity.HasKey(i => i.Id);

                entity.Property(i => i.Quantity)
                    .IsRequired();

                entity.Property(i => i.LastUpdated)
                    .IsRequired();
            });

            // Configuração de Payment
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payments");
                entity.HasKey(p => p.Id);

                entity.Property(p => p.PaymentMethod)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(p => p.TransactionId)
                    .HasMaxLength(100);

                entity.Property(p => p.Amount)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                entity.Property(p => p.Status)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(p => p.PaymentDate)
                    .IsRequired();
            });

            // Configuração de SucCategory
            modelBuilder.Entity<SubCategory>(entity =>
            {
                entity.ToTable("SubCategories");
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(c => c.Description)
                    .HasMaxLength(500);

                // Relacionamentos
                entity.HasMany(c => c.CategorySubCategories)
                    .WithOne(pc => pc.SubCategory)
                    .HasForeignKey(pc => pc.SubCategoryId);
            });

            // Configuração de ProductCategory
            modelBuilder.Entity<CategorySubCategory>(entity =>
            {
                entity.ToTable("CategorySubCategories");
                entity.HasKey(pc => new { pc.CategoryId, pc.SubCategoryId });
            });

            // Configuração de Brand
            modelBuilder.Entity<Brand>(entity =>
            {
                entity.ToTable("Brands");
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(c => c.Description)
                    .HasMaxLength(500);

                // Relacionamentos
                entity.HasMany(c => c.ProductBrands)
                    .WithOne(pc => pc.Brand)
                    .HasForeignKey(pc => pc.BrandId);
            });

            // Configuração de ProductBrand
            modelBuilder.Entity<ProductBrand>(entity =>
            {
                entity.ToTable("ProductBrands");
                entity.HasKey(pc => new { pc.ProductId, pc.BrandId });
            });

            // Configuração de Objective
            modelBuilder.Entity<Objective>(entity =>
            {
                entity.ToTable("Objectives");
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(c => c.Description)
                    .HasMaxLength(500);

                // Relacionamentos
                entity.HasMany(c => c.ProductObjectives)
                    .WithOne(pc => pc.Objective)
                    .HasForeignKey(pc => pc.ObjectiveId);
            });

            // Configuração de ProductObjective
            modelBuilder.Entity<ProductObjective>(entity =>
            {
                entity.ToTable("ProductObjectives");
                entity.HasKey(pc => new { pc.ProductId, pc.ObjectiveId });
            });
        }
    }
}