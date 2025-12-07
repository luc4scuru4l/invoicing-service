using Invoicing.Domain.Entities.Base;

namespace Invoicing.Domain.Entities;

public class TaxRule : TenantEntity
{
  public Guid Id { get; private set; }
  public byte ProductTaxCategoryId { get; private set; }
  public byte ClientTaxCategoryId { get; private set; }
  public Guid TaxId { get; private set; }

  private TaxRule() { }
}