using Invoicing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invoicing.Infrastructure.Persistence.Configurations;

public class PointOfSaleCounterConfiguration : IEntityTypeConfiguration<PointOfSaleCounter>
{
  public void Configure(EntityTypeBuilder<PointOfSaleCounter> builder)
  {
    builder.ToTable("PointOfSaleCounters");

    builder.HasKey(x => new { x.PointOfSaleId, x.ArcaVoucherCode });

    builder.Property(x => x.LastNumber).IsConcurrencyToken();

    builder.HasOne<Tenant>()
      .WithMany()
      .HasForeignKey(x => x.TenantId)
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne<PointOfSale>()
           .WithMany()
           .HasForeignKey(x => x.PointOfSaleId)
           .OnDelete(DeleteBehavior.Restrict);
  }
}