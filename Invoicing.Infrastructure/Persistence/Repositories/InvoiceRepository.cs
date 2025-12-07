using Invoicing.Domain.Entities;
using Invoicing.Application.Interfaces.Repositories;
using Invoicing.Infrastructure.Persistence.Contexts;

namespace Invoicing.Infrastructure.Persistence.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
  private readonly InvoicingDbContext _context;

  public InvoiceRepository(InvoicingDbContext context)
  {
    _context = context;
  }

  public async Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default)
  {
    await _context.Invoices.AddAsync(invoice, cancellationToken);
  }
}
