using System;

namespace Invoicing.Core.Interfaces;

public interface ITenantEntity
{
    Guid TenantId { get; }
}