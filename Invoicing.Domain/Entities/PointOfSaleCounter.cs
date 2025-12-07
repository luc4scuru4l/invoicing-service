using Invoicing.Domain.Entities.Base;

namespace Invoicing.Domain.Entities;

public class PointOfSaleCounter : TenantEntity
{
  public Guid PointOfSaleId { get; private set; }
  public int ArcaVoucherCode { get; private set; }
  public long LastNumber { get; private set; }
  public DateTimeOffset LastIssueDate { get; private set; }

  public PointOfSaleCounter(Guid pointOfSaleId, int arcaVoucherCode, long lastNumber, DateTimeOffset lastIssueDate, Guid tenantId) : base(tenantId)
  {
    PointOfSaleId = pointOfSaleId;
    ArcaVoucherCode = arcaVoucherCode;
    LastNumber = lastNumber;
    LastIssueDate = lastIssueDate;
  }

  public void IncreaseNumber()
  {
    LastNumber++;
  }

  public void UpdateLastNumber(long newNumber)
  {
    if (newNumber <= LastNumber) throw new InvalidOperationException($"El nuevo número ({newNumber}) debe ser mayor al actual ({LastNumber})");
    LastNumber = newNumber;
  }

  private PointOfSaleCounter() { }
}
