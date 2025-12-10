using Invoicing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invoicing.Infrastructure.Persistence.Configurations;

public class AuthorizationRequestConfiguration : IEntityTypeConfiguration<AuthorizationRequest>
{
  public void Configure(EntityTypeBuilder<AuthorizationRequest> builder)
  {
    builder.ToTable("AuthorizationRequests");

    builder.HasKey(x => x.IdempotencyKey);
    builder.Property(x => x.IdempotencyKey).ValueGeneratedNever();

    builder.Property(x => x.Status).HasMaxLength(20).IsRequired();
    builder.Property(x => x.RequestPayload).HasColumnType("nvarchar(max)").IsRequired();
    builder.Property(x => x.FailureReason).HasColumnType("nvarchar(max)"); // Nullable
    builder.Property(x => x.DocumentDescription).HasMaxLength(50).IsRequired();

    builder.HasOne<PointOfSale>()
      .WithMany()
      .HasForeignKey(x => x.PointOfSaleId)
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne<Tenant>()
      .WithMany()
      .HasForeignKey(x => x.TenantId)
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasIndex(x => new { x.TenantId, x.UserId, x.Status });
  }
}