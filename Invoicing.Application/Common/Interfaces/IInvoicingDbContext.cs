using Invoicing.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Invoicing.Application.Common.Interfaces;

public interface IInvoicingDbContext
{
  DbSet<Invoice> Invoices { get; }
  DbSet<PointOfSale> PointOfSales { get; }
  DbSet<PointOfSaleCounter> PointOfSaleCounters { get; }
  DbSet<Product> Products { get; }
  DbSet<Client> Clients { get; }
  DbSet<TaxRule> TaxRules { get; }
  DbSet<Tax> Taxes { get; }

  Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}