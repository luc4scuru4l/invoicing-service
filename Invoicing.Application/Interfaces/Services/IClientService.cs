using System;
using System.Threading;
using System.Threading.Tasks;
using Invoicing.Core.Models;

namespace Invoicing.Core.Interfaces.Services;

public interface IClientService
{
    Task<ClientInfo?> GetClientAsync(Guid clientId, CancellationToken cancellationToken = default);
}