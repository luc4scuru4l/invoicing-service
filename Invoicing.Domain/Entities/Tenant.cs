namespace Invoicing.Domain.Entities;

public class Tenant
{
  public Guid Id { get; private set; }
  public string Name { get; private set; }
  public string TaxId { get; private set; }
  public string FiscalAddress { get; private set; }
  public DateTimeOffset CreatedAtUtc { get; private set; }

  public Tenant(Guid id, string name, string taxId, string fiscalAddress)
  {
    if (id == Guid.Empty)
      throw new ArgumentException("Id no puede estar vacío.", nameof(id));
    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException("Name no puede estar vacío.", nameof(name));
    if (string.IsNullOrWhiteSpace(taxId))
      throw new ArgumentException("TaxId no puede estar vacío.", nameof(taxId));

    Id = id;
    Name = name;
    TaxId = taxId;
    FiscalAddress = fiscalAddress ?? string.Empty;
    CreatedAtUtc = DateTimeOffset.UtcNow;
  }

  public void UpdateFiscalData(string name, string fiscalAddress)
  {
    Name = name;
    FiscalAddress = fiscalAddress;
  }

  private Tenant()
  {
    Name = null!;
    TaxId = null!;
    FiscalAddress = null!;
  }
}