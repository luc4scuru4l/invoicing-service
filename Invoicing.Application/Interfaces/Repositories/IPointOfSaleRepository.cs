namespace Invoicing.Application.Interfaces.Repositories;

public interface IPointOfSaleRepository
{
  Task<long> GetNextInvoiceNumberAsync(Guid pointOfSaleId, int arcaVoucherCode, CancellationToken cancellationToken = default);

  Task UpdateLastNumberAsync(Guid pointOfSaleId, int arcaVoucherCode, long newNumber, CancellationToken cancellationToken = default);
}
