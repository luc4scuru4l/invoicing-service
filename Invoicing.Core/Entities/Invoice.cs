using System;
using System.Collections.Generic;
using Invoicing.Core.Entities.Base;

namespace Invoicing.Core.Entities;

public class Invoice : TenantEntity
{
    public Guid Id { get; private set; }
    public Guid ClientId { get; private set; }
    public Guid PointOfSaleId { get; private set; }
    public Guid CreatedByUserId { get; private set; }

    public int ArcaVoucherCode { get; private set; } // Ej: 1 (Factura A), 6 (Factura B)
    public long InvoiceNumber { get; private set; }  // Ej: 12345
    public string? Cae { get; private set; }
    public DateTimeOffset? CaeDueDate { get; private set; }

    public decimal TotalAmount { get; private set; }
    public decimal NetAmount { get; private set; }
    public decimal TaxAmount { get; private set; }
    
    public string? Currency { get; private set; } // "ARS", "USD"
    public string? PaymentCurrency { get; private set; } // "ARS", "USD"
    
    // Fechas
    public DateTimeOffset IssueDate { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    // Colección de Ítems (Relación 1 a muchos)
    private readonly List<InvoiceItem> _items = new();
    public IReadOnlyCollection<InvoiceItem> Items => _items.AsReadOnly();

    // Constructor para una factura NUEVA (Borrador / Pre-autorización)
    public Invoice(Guid tenantId, Guid clientId, Guid pointOfSaleId, Guid createdByUserId, string currency, DateTimeOffset issueDate) 
    : base(tenantId)
    {
      if (createdByUserId == Guid.Empty) throw new ArgumentException("UserId no puede estar vacío.", nameof(createdByUserId));
      if (clientId == Guid.Empty) throw new ArgumentException("ClientId no puede estar vacío.", nameof(clientId));
      if (pointOfSaleId == Guid.Empty) throw new ArgumentException("PointOfSaleId no puede estar vacío.", nameof(pointOfSaleId));
      
      if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency no puede estar vacío.", nameof(currency));
      if (issueDate < DateTimeOffset.UtcNow.AddYears(-2) || issueDate > DateTimeOffset.UtcNow.AddYears(2))
        throw new ArgumentException("IssueDate debe ser una fecha válida", nameof(issueDate));
      
      Id = Guid.NewGuid();
      ClientId = clientId;
      PointOfSaleId = pointOfSaleId;
      CreatedByUserId = createdByUserId;
      Currency = currency;
      IssueDate = issueDate;
      CreatedAtUtc = DateTimeOffset.UtcNow;
    }

    // Método de Negocio: Autorizar la factura (Setear datos de AFIP)
    public void Authorize(long number, int arcaCode, string cae, DateTimeOffset caeDueDate)
    {
        // Aquí podríamos validar que no esté ya autorizada
        InvoiceNumber = number;
        ArcaVoucherCode = arcaCode;
        Cae = cae;
        CaeDueDate = caeDueDate;
    }

    // Método de Negocio: Agregar Ítems y recalcular totales
    public void AddItem(string description, decimal quantity, decimal unitPrice, decimal taxAmount, Guid productId)
    {
        var item = new InvoiceItem(Id, TenantId, productId, description, quantity, unitPrice, taxAmount);
        _items.Add(item);
        CalculateTotals();
    }

    private void CalculateTotals()
    {
        // Lógica simple de suma (luego mejoraremos esto con el motor de impuestos)
        decimal totalItems = 0;
        decimal totalTax = 0;

        foreach (var item in _items)
        {
            totalItems += item.Subtotal;
            totalTax += item.TaxAmount;
        }

        NetAmount = totalItems; // Asumiendo neto por ahora
        TaxAmount = totalTax;
        TotalAmount = NetAmount + TaxAmount;
    }

    // Constructor vacío para EF Core
    private Invoice() { }
}