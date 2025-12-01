using System.Threading;
using System.Threading.Tasks;
using Invoicing.Core.Entities;

namespace Invoicing.Core.Interfaces.Repositories;

public interface IInvoiceRepository
{
    Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default);
}