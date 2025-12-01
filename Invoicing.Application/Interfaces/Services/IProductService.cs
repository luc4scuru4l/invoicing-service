using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Invoicing.Application.Models;

namespace Invoicing.Application.Interfaces.Services;

public interface IProductService
{
    Task<IEnumerable<ProductInfo>> GetProductsAsync(IEnumerable<Guid> productIds, CancellationToken cancellationToken = default);
}