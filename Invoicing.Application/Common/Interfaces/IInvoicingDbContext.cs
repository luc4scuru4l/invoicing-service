using Invoicing.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Invoicing.Application.Common.Interfaces;

public interface IInvoicingDbContext
{
  DbSet<Invoice> Invoices { get; }
  DbSet<PointOfSale> PointOfSales { get; }

  Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}