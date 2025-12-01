using System;
using System.Threading;
using System.Threading.Tasks;

namespace Invoicing.Application.Interfaces.Repositories;

public interface IPointOfSaleRepository
{
    // Necesitamos esto para el UPDLOCK
    Task<long> GetNextInvoiceNumberAsync(Guid pointOfSaleId, int arcaVoucherCode, CancellationToken cancellationToken = default);
    
    // Método para actualizar el contador después del éxito
    Task UpdateLastNumberAsync(Guid pointOfSaleId, int arcaVoucherCode, long newNumber, CancellationToken cancellationToken = default);
}