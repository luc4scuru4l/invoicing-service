using Invoicing.Domain.Entities.Base;

namespace Invoicing.Domain.Entities;

public class InvoiceItem : TenantEntity
{
  public Guid Id { get; private set; }
  public Guid InvoiceId { get; private set; }
  public Guid ProductId { get; private set; }
  public int LineNumber { get; private set; }
  public decimal Quantity { get; private set; }
  public decimal UnitPrice { get; private set; }
  public string Description { get; private set; }
  public decimal Subtotal { get; private set; }
  public decimal TaxAmount { get; private set; }

  public InvoiceItem(Guid invoiceId, Guid tenantId, int lineNumber, Guid productId, string description, decimal quantity, decimal unitPrice, decimal taxAmount) : base(tenantId)
  {
    if (invoiceId == Guid.Empty) throw new ArgumentException("InvoiceId no puede estar vacío.", nameof(invoiceId));
    if (productId == Guid.Empty) throw new ArgumentException("ProductId no puede estar vacío.", nameof(productId));

    if (lineNumber <= 0)
      throw new ArgumentOutOfRangeException(nameof(lineNumber), lineNumber,
        $"LineNumber debe ser mayor a cero. Valor recibido {lineNumber}");
    if (quantity <= 0)
      throw new ArgumentOutOfRangeException(nameof(quantity), quantity, 
        $"Quantity debe ser mayor a cero. Valor recibido: {quantity}");
    if (unitPrice <= 0) 
      throw new ArgumentOutOfRangeException(nameof(unitPrice), unitPrice,
        $"UnitPrice debe ser mayor a cero. Valor recibido {unitPrice}");
    if (taxAmount < 0)
      throw new ArgumentOutOfRangeException(nameof(taxAmount), taxAmount,
        $"TaxAmount debe ser mayor a cero. Valor recibido {taxAmount}");

    if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Description no puede estar vacío.", nameof(description));

    Id = Guid.NewGuid();
    InvoiceId = invoiceId;
    LineNumber = lineNumber;
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
