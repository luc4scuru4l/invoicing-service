using Invoicing.Application.Interfaces.Repositories;
using Invoicing.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Invoicing.Infrastructure.Persistence.Repositories;

public class PointOfSaleRepository : IPointOfSaleRepository
{
  private readonly InvoicingDbContext _context;

  public PointOfSaleRepository(InvoicingDbContext context)
  {
    _context = context;
  }

  public async Task<long> GetNextInvoiceNumberAsync(Guid pointOfSaleId, int arcaVoucherCode, CancellationToken cancellationToken = default)
  {
    var counter = await _context.PointOfSaleCounters
      .FromSqlInterpolated($@"
            SELECT * FROM PointOfSaleCounters WITH (UPDLOCK, ROWLOCK)
            WHERE PointOfSaleId = {pointOfSaleId} 
              AND ArcaVoucherCode = {arcaVoucherCode}
        ")
      .FirstOrDefaultAsync(cancellationToken);

    if (counter == null)
    {
      throw new InvalidOperationException($"No existe un contador configurado para el POS {pointOfSaleId} y Código {arcaVoucherCode}");
    }

    counter.IncreaseNumber();

    return counter.LastNumber;
  }

  public async Task UpdateLastNumberAsync(Guid pointOfSaleId, int arcaVoucherCode, long newNumber, CancellationToken cancellationToken = default)
  {
    var counter = await _context.PointOfSaleCounters
        .FirstOrDefaultAsync(x => x.PointOfSaleId == pointOfSaleId && x.ArcaVoucherCode == arcaVoucherCode, cancellationToken);

    if (counter == null)
    {
      throw new InvalidOperationException($"Counter not found for POS {pointOfSaleId} and Code {arcaVoucherCode}");
    }

    counter.UpdateLastNumber(newNumber);
  }

  public Task UpdateLastNumberAsync()
  {
    return Task.CompletedTask;
  }
}
