using Invoicing.Domain.Entities.Base;

namespace Invoicing.Domain.Entities;

public class PointOfSale : TenantEntity
{
  public Guid Id { get; private set; }
  public int Number { get; private set; }
  public string Name { get; private set; }
  public bool IsDisabled { get; private set; }

  public PointOfSale(Guid tenantId, int number, string name) : base(tenantId)
  {
    Id = Guid.NewGuid();
    Number = number;
    Name = name;
    IsDisabled = false;
  }

  private PointOfSale() { Name = null!; }
}