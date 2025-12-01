using Invoicing.Domain.Entities.Base;

namespace Invoicing.Domain.Entities;

public class InvoiceItem : TenantEntity
{
  public Guid Id { get; private set; }
  public Guid InvoiceId { get; private set; }
  public Guid ProductId { get; private set; }
  public decimal Quantity { get; private set; }
  public decimal UnitPrice { get; private set; }
  public string Description {get; private set;}
  public decimal Subtotal {get; private set;}
  public decimal TaxAmount {get; private set;}

  public InvoiceItem(Guid invoiceId, Guid tenantId, Guid productId, string description, decimal quantity, decimal unitPrice, decimal taxAmount) : base(tenantId) 
  {
    if (invoiceId == Guid.Empty) throw new ArgumentException("InvoiceId no puede estar vacío.", nameof(invoiceId));
    if (productId == Guid.Empty) throw new ArgumentException("ProductId no puede estar vacío.", nameof(productId));
    
    if (quantity <= 0) throw new ArgumentException("Quantity debe ser un numero mayor a cero.", nameof(quantity));
    if (unitPrice <= 0) throw new ArgumentException("UnitPrice debe ser un numero mayor a cero.", nameof(unitPrice));
    if (taxAmount < 0) throw new ArgumentException("TaxAmount debe ser un numero positivo.", nameof(unitPrice));

    if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Description no puede estar vacío.", nameof(description));

    Id = Guid.NewGuid();
    InvoiceId = invoiceId;
    ProductId = productId;
    Quantity = quantity;
    UnitPrice = unitPrice; 
    Description = description;
    TaxAmount = taxAmount;

    Subtotal = Quantity * UnitPrice;
  }

  private InvoiceItem()
  {
    Description = null!;
  }
}