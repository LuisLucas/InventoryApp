using InventoryAPI.Application.Common;
using InventoryAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryApi.Infrastructure.Data;

public partial class InventoryContext : DbContext, IDbContext
{
    public InventoryContext()
    {
    }

    public InventoryContext(DbContextOptions<InventoryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Stock> Stocks { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Product");

            entity.ToTable("Product");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(1);
            entity.Property(e => e.Description).HasMaxLength(1);
            entity.Property(e => e.LastUpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.LastUpdatedBy).HasMaxLength(1);
            entity.Property(e => e.Name).HasMaxLength(1);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Sku)
                .HasMaxLength(1)
                .HasColumnName("SKU");
        });

        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Stock");

            entity.ToTable("Stock");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(1);
            entity.Property(e => e.LastUpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.LastUpdatedBy).HasMaxLength(1);
            entity.Property(e => e.ProductId).HasColumnName("Product_Id");

            entity.HasOne(d => d.Product).WithMany(p => p.Stocks)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Stock__Product_I__398D8EEE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
