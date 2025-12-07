using Invoicing.Application.Interfaces.Repositories;
using Invoicing.Domain.Entities; // Asumiendo que tenés la entidad PointOfSaleCounter
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
      // LA LÓGICA DE BLOQUEO (UPDLOCK + ROWLOCK)
      // Usamos FromSqlInterpolated para evitar SQL Injection automáticamente.
      // "LastNumber" es lo que queremos leer.
      
      var counter = await _context.PointOfSaleCounters
        .FromSqlInterpolated($@"
            SELECT * FROM PointOfSaleCounters WITH (UPDLOCK, ROWLOCK)
            WHERE PointOfSaleId = {pointOfSaleId} 
              AND ArcaVoucherCode = {arcaVoucherCode}
        ")
        .FirstOrDefaultAsync(cancellationToken);

      if (counter == null)
      {
        // Opcional: Podrías crear el contador al vuelo si no existe, 
        // o lanzar una excepción de configuración.
        throw new InvalidOperationException($"No existe un contador configurado para el POS {pointOfSaleId} y Código {arcaVoucherCode}");
      }

      // Devolvemos el siguiente (Actual + 1)
      // Nota: NO actualizamos la BD todavía. Solo calculamos en memoria.
      // El UnitOfWork guardará el cambio más tarde.
      counter.IncreaseNumber(); // Asumiendo que agregás este método a tu Entidad
      
      return counter.LastNumber; 
  }
    
    public async Task UpdateLastNumberAsync(Guid pointOfSaleId, int arcaVoucherCode, long newNumber, CancellationToken cancellationToken = default)
    {
      // 1. Buscamos la entidad (Como estamos en la misma transacción y UnitOfWork, 
      // es muy probable que EF Core ya la tenga en memoria caché "Local", por lo que no hace una query real).
      var counter = await _context.PointOfSaleCounters
          .FirstOrDefaultAsync(x => x.PointOfSaleId == pointOfSaleId && x.ArcaVoucherCode == arcaVoucherCode, cancellationToken);

      if (counter == null)
      {
        throw new InvalidOperationException($"Counter not found for POS {pointOfSaleId} and Code {arcaVoucherCode}");
      }

      // 2. Modificamos la entidad usando su método de dominio
      counter.UpdateLastNumber(newNumber);

      // 3. NO HACEMOS SaveChanges(). 
      // Al modificar la entidad traída del contexto, EF Core marca el estado como "Modified".
      // El UnitOfWork se encargará de impactar el cambio en la BD cuando hagamos Commit.
    }

    // Este método es opcional si usas EF Core Change Tracking, 
    // porque al traerlo con 'FromSql', EF ya lo está traqueando.
    // Si modificás la entidad 'counter', EF detectará el cambio solo.
    public Task UpdateLastNumberAsync() 
    {
      return Task.CompletedTask;
    }
}