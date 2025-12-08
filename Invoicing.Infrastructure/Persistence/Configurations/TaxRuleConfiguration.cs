using Invoicing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invoicing.Infrastructure.Persistence.Configurations;

public class TaxRuleConfiguration : IEntityTypeConfiguration<TaxRule>
{
  public void Configure(EntityTypeBuilder<TaxRule> builder)
  {
    builder.ToTable("TaxRules");

    builder.HasKey(x => x.Id);

    builder.Property(x => x.ProductTaxCategoryId).IsRequired();
    builder.Property(x => x.ClientTaxCategoryId).IsRequired();

    builder.HasOne<Tax>()
      .WithMany()
      .HasForeignKey(x => x.TaxId)
      .OnDelete(DeleteBehavior.Restrict);
    
    builder.HasIndex(x => new { x.TenantId, x.ProductTaxCategoryId, x.ClientTaxCategoryId })
      .IsUnique();
  }
}