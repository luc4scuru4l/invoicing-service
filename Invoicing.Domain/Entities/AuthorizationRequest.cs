using Invoicing.Domain.Entities.Base;

namespace Invoicing.Domain.Entities;

public class AuthorizationRequest : TenantEntity
{
  public Guid IdempotencyKey { get; private set; }
  public string Status { get; private set; } // 'Pending', 'Success', 'Failed'
  public string RequestPayload { get; private set; } // JSON
  public string? FailureReason { get; private set; }

  // Auditoría del intento
  public Guid UserId { get; private set; }
  public DateTimeOffset CreatedAtUtc { get; private set; }

  // Datos para reconstruir el contexto del intento
  public Guid PointOfSaleId { get; private set; }
  public int ArcaVoucherCode { get; private set; }
  public string DocumentDescription { get; private set; } // "Factura A"

  public Guid? AuthorizedDocumentId { get; private set; }

  public AuthorizationRequest(
      Guid idempotencyKey,
      Guid tenantId,
      Guid userId,
      Guid pointOfSaleId,
      int arcaVoucherCode,
      string documentDescription,
      string requestPayload) : base(tenantId)
  {
    if (idempotencyKey == Guid.Empty) throw new ArgumentException("El id de idempotencia es requerido.", nameof(idempotencyKey));

    IdempotencyKey = idempotencyKey;
    UserId = userId;
    PointOfSaleId = pointOfSaleId;
    ArcaVoucherCode = arcaVoucherCode;
    DocumentDescription = documentDescription;
    RequestPayload = requestPayload;

    Status = "Pending";
    CreatedAtUtc = DateTimeOffset.UtcNow;
  }

  public void MarkAsSuccess(Guid documentId)
  {
    Status = "Success";
    AuthorizedDocumentId = documentId;
  }

  public void MarkAsFailed(string reason)
  {
    Status = "Failed";
    FailureReason = reason;
  }

  private AuthorizationRequest()
  {
    Status = null!;
    RequestPayload = null!;
    DocumentDescription = null!;
  }
}