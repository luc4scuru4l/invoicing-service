using Invoicing.Domain.Entities.Base;

namespace Invoicing.Domain.Entities;

public class Product : TenantEntity
{
  public Guid Id { get; private set; }
  public string Name { get; private set; }

  // 1 = Bienes Generales, 2 = Servicios, 3 = Digitales, etc.
  public byte TaxCategoryId { get; private set; }
  public decimal UnitPrice { get; private set; }

  private Product()
  {
    Name = null!;
  }

  public Product(Guid id, Guid tenantId, string name, byte taxCategoryId, decimal unitPrice) : base(tenantId)
  {
    if (id == Guid.Empty) throw new ArgumentException("Id no puede estar vacío.", nameof(id));

    if (unitPrice <= 0)
      throw new ArgumentOutOfRangeException(nameof(unitPrice), unitPrice,
        $"UnitPrice debe ser mayor a cero. Valor recibido {unitPrice}");

    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException("Name no puede estar vacío.", nameof(name));

    if (taxCategoryId <= 0)
      throw new ArgumentException("TaxCategoryId no puede estar vacío.", nameof(taxCategoryId));

    Id = id;
    Name = name;
    TaxCategoryId = taxCategoryId;
    UnitPrice = unitPrice;
  }
}