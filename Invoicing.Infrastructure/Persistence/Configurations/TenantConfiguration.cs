using Invoicing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invoicing.Infrastructure.Persistence.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
  public void Configure(EntityTypeBuilder<Tenant> builder)
  {
    builder.ToTable("Tenants");

    builder.HasKey(t => t.Id);

    builder.Property(t => t.Id).ValueGeneratedNever();

    builder.Property(t => t.Name)
        .HasMaxLength(100)
        .IsRequired();

    builder.Property(t => t.TaxId)
        .HasMaxLength(20)
        .IsRequired();

    builder.Property(t => t.FiscalAddress)
        .HasMaxLength(300);

    builder.HasIndex(t => t.TaxId);
  }
}