using Invoicing.Application.Interfaces;
using Invoicing.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore.Storage;

namespace Invoicing.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
  private readonly InvoicingDbContext _context;
  private IDbContextTransaction? _currentTransaction;

  public UnitOfWork(InvoicingDbContext context)
  {
    _context = context;
  }

  public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
  {
    if (_currentTransaction != null) return;

    _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
  }

  public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
  {
    try
    {
      // Guardamos los cambios en memoria (INSERTs, UPDATEs)
      await _context.SaveChangesAsync(cancellationToken);

      // Confirmamos la transacción de base de datos
      if (_currentTransaction != null)
      {
        await _currentTransaction.CommitAsync(cancellationToken);
      }
    }
    catch
    {
      await RollbackTransactionAsync(cancellationToken);
      throw;
    }
    finally
    {
      if (_currentTransaction != null)
      {
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
      }
    }
  }

  public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
  {
    if (_currentTransaction != null)
    {
      await _currentTransaction.RollbackAsync(cancellationToken);
      await _currentTransaction.DisposeAsync();
      _currentTransaction = null;
    }
  }

  public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    // Guardado simple sin transacción explícita (útil para tests o pasos intermedios)
    await _context.SaveChangesAsync(cancellationToken);
  }
}
