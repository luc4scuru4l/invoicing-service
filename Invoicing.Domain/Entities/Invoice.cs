using Invoicing.Domain.Entities.Base;

namespace Invoicing.Domain.Entities;

public class Invoice : TenantEntity
{
  public Guid Id { get; private set; }
  public Guid ClientId { get; private set; }
  public Guid PointOfSaleId { get; private set; }
  public Guid CreatedByUserId { get; private set; }
  public long InvoiceNumber { get; private set; }  // Ej: 12345
  public char InvoiceLetter { get; private set; }  // Ej: A
  public int ArcaVoucherCode { get; private set; } // Ej: 1 (Factura A), 6 (Factura B)
  public string? Cae { get; private set; }
  public DateTimeOffset? CaeDueDate { get; private set; }
  public decimal TotalAmount { get; private set; }
  public decimal NetAmount { get; private set; }
  public decimal TaxAmount { get; private set; }
  public string Currency { get; private set; } // "ARS", "USD"
  public string PaymentCurrency { get; private set; } // "ARS", "USD"
  public decimal ExchangeRate { get; private set; }
  public DateTimeOffset IssueDate { get; private set; }
  public DateTimeOffset DueDate { get; private set; }
  public DateTimeOffset CreatedAtUtc { get; private set; }
  private const int MAX_DUE_DAYS = 365;

  private readonly List<InvoiceItem> _items = new();
  public IReadOnlyCollection<InvoiceItem> Items => _items.AsReadOnly();

  public Invoice(Guid tenantId, Guid clientId, Guid pointOfSaleId, Guid createdByUserId, string currency, string paymentCurrency, DateTimeOffset issueDate, DateTimeOffset dueDate)
  : base(tenantId)
  {
    if (createdByUserId == Guid.Empty) throw new ArgumentException("UserId no puede estar vacío.", nameof(createdByUserId));
    if (clientId == Guid.Empty) throw new ArgumentException("ClientId no puede estar vacío.", nameof(clientId));
    if (pointOfSaleId == Guid.Empty) throw new ArgumentException("PointOfSaleId no puede estar vacío.", nameof(pointOfSaleId));

    if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency no puede estar vacío.", nameof(currency));
    if (string.IsNullOrWhiteSpace(paymentCurrency)) throw new ArgumentException("PaymentCurrency no puede estar vacío.", nameof(paymentCurrency));

    if (issueDate < DateTimeOffset.UtcNow.AddYears(-2) || issueDate > DateTimeOffset.UtcNow.AddYears(2))
      throw new ArgumentException("IssueDate debe ser una fecha válida", nameof(issueDate));
    
    if (dueDate < issueDate || dueDate > issueDate.AddDays(MAX_DUE_DAYS))
      throw new ArgumentException($"DueDate debe ser una fecha comprendida entre {issueDate:yyyy-MM-dd} y {issueDate.AddDays(100):yyyy-MM-dd}", nameof(dueDate));

    Id = Guid.NewGuid();
    ClientId = clientId;
    PointOfSaleId = pointOfSaleId;
    CreatedByUserId = createdByUserId;
    Currency = currency;
    PaymentCurrency = paymentCurrency;
    CreatedAtUtc = DateTimeOffset.UtcNow;
    IssueDate = issueDate;
    DueDate = dueDate;
  }

  public void Authorize(long number, int arcaCode, string cae, DateTimeOffset caeDueDate)
  {
    InvoiceNumber = number;
    ArcaVoucherCode = arcaCode;
    Cae = cae;
    CaeDueDate = caeDueDate;
  }

  public void AddItem(string description, decimal quantity, decimal unitPrice, decimal taxAmount, Guid productId)
  {
    int nextNumber = _items.Count + 1;
    var item = new InvoiceItem(Id, TenantId, nextNumber, productId, description, quantity, unitPrice, taxAmount);
    _items.Add(item);
    CalculateTotals();
  }

  private void CalculateTotals()
  {
    decimal totalItems = 0;
    decimal totalTax = 0;

    foreach (var item in _items)
    {
      totalItems += item.Subtotal;
      totalTax += item.TaxAmount;
    }

    NetAmount = totalItems;
    TaxAmount = totalTax;
    TotalAmount = NetAmount + TaxAmount;
  }

  private Invoice()
  {
    Currency = null!;
    PaymentCurrency = null!;
  }
}
