using Invoicing.Domain.Entities.Base;

namespace Invoicing.Domain.Entities;

public class Tax : TenantEntity
{
  public Guid Id { get; private set; }
  public string Name { get; private set; } // "IVA 21%"
  public decimal Rate { get; private set; } // 0.21
  
  private Tax() { Name = null!; }
}