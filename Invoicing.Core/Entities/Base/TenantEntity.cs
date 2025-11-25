using System;
using Invoicing.Core.Interfaces;

namespace Invoicing.Core.Entities.Base;

public abstract class TenantEntity : ITenantEntity
{
    public Guid TenantId { get; private set; }

    protected TenantEntity(Guid tenantId)
    {
      if (tenantId == Guid.Empty) 
          throw new ArgumentException("TenantId no puede estar vacío.", nameof(tenantId));
      TenantId = tenantId;
    }

    protected TenantEntity() { }
}