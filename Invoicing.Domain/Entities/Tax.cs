using Invoicing.Domain.Entities.Base;

namespace Invoicing.Domain.Entities;

public class Tax : TenantEntity
{
  public Guid Id { get; private set; }
  public string Name { get; private set; } // "IVA 21%"
  public decimal Rate { get; private set; } // 0.21

  public Tax(Guid tenantId, string name, decimal rate) : base(tenantId)
  {
    if (rate < 0 || rate > 1)
      throw new ArgumentOutOfRangeException(nameof(rate), rate,
        $"Rate debe ser mayor a cero y menor a 1. Valor recibido {rate}");

    Id = Guid.NewGuid();
    Name = name;
    Rate = rate;
  }

  private Tax() { Name = null!; }
}