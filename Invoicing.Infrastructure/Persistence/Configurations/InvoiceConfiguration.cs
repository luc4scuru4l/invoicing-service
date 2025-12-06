using Invoicing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invoicing.Infrastructure.Persistence.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");
        
        builder.HasKey(i => i.Id);
        
        builder.Property(i => i.TotalAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        builder.Property(i => i.NetAmount)
            .HasColumnType("decimal(18,2)");
        builder.Property(i => i.TaxAmount)
            .HasColumnType("decimal(18,2)");
        builder.Property(i => i.ExchangeRate)
            .HasColumnType("decimal(18,4)");
        builder.Property(i => i.Currency)
            .HasMaxLength(3)
            .IsRequired();
        
        builder.HasMany(i => i.Items)
            .WithOne()
            .HasForeignKey(item => item.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(Invoice.Items))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}