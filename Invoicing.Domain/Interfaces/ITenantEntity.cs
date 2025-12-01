using System;

namespace Invoicing.Domain.Interfaces;

public interface ITenantEntity
{
    Guid TenantId { get; }
}