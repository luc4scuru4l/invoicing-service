using Invoicing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invoicing.Infrastructure.Persistence.Configurations;

public class InvoiceItemTaxConfiguration : IEntityTypeConfiguration<InvoiceItemTax>
{
  public void Configure(EntityTypeBuilder<InvoiceItemTax> builder)
  {
    builder.ToTable("InvoiceItemTax");

    builder.HasKey(x => x.Id);

    builder.Property(i => i.TaxName)
      .HasMaxLength(100)
      .IsRequired();

    builder.Property(x => x.BaseAmount)
      .HasColumnType("decimal(18,2)")
      .IsRequired();
    
    builder.Property(x => x.AppliedAmount)
      .HasColumnType("decimal(18,2)")
      .IsRequired();

    builder.Property(x => x.TaxRate)
      .HasColumnType("decimal(18,6)")
      .IsRequired();

    builder.HasOne<InvoiceItem>()
      .WithMany()
      .HasForeignKey(x => x.InvoiceItemId)
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne<Tax>()
      .WithMany()
      .HasForeignKey(x => x.TaxId)
      .OnDelete(DeleteBehavior.Restrict);
    
    builder.HasOne<Tenant>()
      .WithMany()
      .HasForeignKey(x => x.TenantId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}