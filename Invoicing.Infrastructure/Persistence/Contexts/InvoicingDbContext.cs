using System.Reflection;
using Invoicing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Invoicing.Application.Common.Interfaces;

namespace Invoicing.Infrastructure.Persistence.Contexts;

public class InvoicingDbContext : DbContext, IInvoicingDbContext
{
  public InvoicingDbContext(DbContextOptions<InvoicingDbContext> options) : base(options)
  {
  }

  public DbSet<Invoice> Invoices { get; set; }
  public DbSet<InvoiceItem> InvoiceItems { get; set; }
  public DbSet<InvoiceItemTax> InvoiceItemTaxes { get; set; }

  public DbSet<PointOfSale> PointOfSales { get; set; }
  public DbSet<PointOfSaleCounter> PointOfSaleCounters { get; set; }

  public DbSet<AuthorizationRequest> AuthorizationRequests { get; set; }

  public DbSet<Product> Products { get; set; }
  public DbSet<Client> Clients { get; set; }
  public DbSet<Tax> Taxes { get; set; }
  public DbSet<TaxRule> TaxRules { get; set; }

  public DbSet<Tenant> Tenants { get; set; }
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
