using Invoicing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invoicing.Infrastructure.Persistence.Configurations;

public class TaxConfiguration : IEntityTypeConfiguration<Tax>
{
  public void Configure(EntityTypeBuilder<Tax> builder)
  {
    builder.ToTable("Taxes");

    builder.HasKey(x => x.Id);

    builder.Property(i => i.Name)
      .HasMaxLength(100)
      .IsRequired();

    builder.Property(x => x.Rate)
      .HasColumnType("decimal(18,6)")
      .IsRequired();
    
    builder.HasOne<Tenant>()
      .WithMany()
      .HasForeignKey(x => x.TenantId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}