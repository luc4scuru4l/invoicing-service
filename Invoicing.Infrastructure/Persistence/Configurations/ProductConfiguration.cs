using Invoicing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invoicing.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
  public void Configure(EntityTypeBuilder<Product> builder)
  {
    builder.ToTable("Products");

    builder.HasKey(x => x.Id);
    builder.Property(x => x.Id)
      .ValueGeneratedNever();

    builder.Property(x => x.Name)
      .HasMaxLength(200)
      .IsRequired();

    // Categoría Fiscal (1, 2, 3...)
    builder.Property(x => x.TaxCategoryId)
      .IsRequired();

    builder.Property(x => x.UnitPrice)
      .HasColumnType("decimal(18,4)")
      .IsRequired();

    builder.Property(x => x.TenantId)
           .IsRequired();

    builder.HasIndex(x => x.TenantId);
  }
}