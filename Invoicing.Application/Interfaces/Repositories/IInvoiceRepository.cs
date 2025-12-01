using System.Threading;
using System.Threading.Tasks;
using Invoicing.Domain.Entities;

namespace Invoicing.Application.Interfaces.Repositories;

public interface IInvoiceRepository
{
    Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default);
}