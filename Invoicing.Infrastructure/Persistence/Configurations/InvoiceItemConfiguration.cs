using Invoicing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invoicing.Infrastructure.Persistence.Configurations;

public class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
{
  public void Configure(EntityTypeBuilder<InvoiceItem> builder)
  {
    builder.ToTable("InvoiceItems");
    builder.HasKey(x => x.Id);

    builder.Property(x => x.Quantity)
      .HasColumnType("decimal(18,4)")
      .IsRequired();

    builder.Property(x => x.UnitPrice)
      .HasColumnType("decimal(18,4)")
      .IsRequired();

    builder.Property(x => x.Subtotal)
      .HasColumnType("decimal(18,2)")
      .IsRequired();

    builder.Property(x => x.TaxAmount)
      .HasColumnType("decimal(18,2)")
      .IsRequired();

    builder.Property(x => x.Description)
      .HasMaxLength(255)
      .IsRequired();

    builder.HasIndex(x => new { x.InvoiceId, x.LineNumber })
      .IsUnique();

    builder.HasMany<InvoiceItemTax>()
      .WithOne()
      .HasForeignKey(x => x.InvoiceItemId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}