namespace Invoicing.Application.Models;

public class ClientInfo
{
  public Guid Id { get; set; }
  public Guid TenantId { get; set; }
  public byte TaxCategoryId { get; set; }
  public string Cuit { get; set; } = string.Empty;
}
