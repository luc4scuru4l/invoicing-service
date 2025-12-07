using System.Reflection;
using Invoicing.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Invoicing.Infrastructure.Persistence.Contexts;

public class InvoicingDbContext : DbContext
{
  public InvoicingDbContext(DbContextOptions<InvoicingDbContext> options) : base(options)
  {
  }

  public DbSet<Invoice> Invoices { get; set; }
  public DbSet<InvoiceItem> InvoiceItems { get; set; }
  public DbSet<PointOfSaleCounter> PointOfSaleCounters { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
