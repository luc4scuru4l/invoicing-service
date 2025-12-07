using Invoicing.Application.Models;

namespace Invoicing.Application.Interfaces.Services;

public interface IClientService
{
  Task<ClientInfo?> GetClientAsync(Guid clientId, CancellationToken cancellationToken = default);
}
