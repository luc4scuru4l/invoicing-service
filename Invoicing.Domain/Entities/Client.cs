using Invoicing.Domain.Entities.Base;

namespace Invoicing.Domain.Entities;

public class Client : TenantEntity
{
  public Guid Id { get; private set; }
  public string Name { get; private set; }

  public string Cuit { get; private set; }
  // 1 = Responsable Inscripto, 2 = Consumidor Final, 3 = Exento
  public byte TaxCategoryId { get; private set; }

  public Client(Guid id, Guid tenantId, string name, string cuit, byte taxCategoryId) : base(tenantId)
  {
    if (id == Guid.Empty) throw new ArgumentException("Id no puede estar vacío.", nameof(id));
    
    if (string.IsNullOrWhiteSpace(cuit)) throw new ArgumentException("Cuit no puede estar vacío.", nameof(id));

    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException("Name no puede estar vacío.", nameof(name));

    if (taxCategoryId <= 0)
      throw new ArgumentException("TaxCategoryId no puede estar vacío.", nameof(taxCategoryId));

    Id = id;
    Name = name;
    Cuit = cuit;
    TaxCategoryId = taxCategoryId;
  }

  private Client()
  {
    Name = null!;
    Cuit = null!;
  }
}