using Invoicing.Domain.Entities.Base;

namespace Invoicing.Domain.Entities;

public class InvoiceItemTax : TenantEntity
{
  public Guid Id { get; private set; }
  public Guid InvoiceItemId { get; private set; }
  public Guid TaxId { get; private set; }

  // Snapshot histórico
  public string TaxName { get; private set; }
  public decimal TaxRate { get; private set; }
  public decimal BaseAmount { get; private set; }
  public decimal AppliedAmount { get; private set; }

  public InvoiceItemTax(Guid tenantId, Guid invoiceItemId, Guid taxId, string taxName, decimal taxRate, decimal baseAmount, decimal appliedAmount) : base(tenantId)
  {
    Id = Guid.NewGuid();
    InvoiceItemId = invoiceItemId;
    TaxId = taxId;
    TaxName = taxName;
    TaxRate = taxRate;
    BaseAmount = baseAmount;
    AppliedAmount = appliedAmount;
  }

  private InvoiceItemTax() { TaxName = null!; }
}