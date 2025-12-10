using Invoicing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invoicing.Infrastructure.Persistence.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
  public void Configure(EntityTypeBuilder<Client> builder)
  {
    builder.ToTable("Clients");

    builder.HasKey(x => x.Id);
    
    builder.Property(x => x.Id)
      .ValueGeneratedNever();

    builder.Property(x => x.Name)
      .HasMaxLength(200)
      .IsRequired();

    // Categoría Fiscal (Resp Inscripto, Cons Final, etc)
    builder.Property(x => x.TaxCategoryId)
      .IsRequired();

    builder.Property(x => x.TenantId)
      .IsRequired();

    builder.HasIndex(x => x.TenantId);
  }
}