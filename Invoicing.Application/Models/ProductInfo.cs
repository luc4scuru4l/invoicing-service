namespace Invoicing.Application.Models;

public class ProductInfo
{
  public Guid Id { get; set; }
  public Guid TenantId { get; set; }
  public string Description { get; set; } = string.Empty;
  public decimal UnitPrice { get; set; }
  public byte TaxCategoryId { get; set; }
}
