using Invoicing.Application.Interfaces;
using Invoicing.Application.Interfaces.Repositories;
using Invoicing.Infrastructure.Persistence;
using Invoicing.Infrastructure.Persistence.Contexts;
using Invoicing.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Invoicing.Infrastructure;

public static class DependencyInjection
{
  public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
  {    
    var connectionString = configuration.GetConnectionString("DefaultConnection");

    services.AddDbContext<InvoicingDbContext>(options =>
        options.UseSqlServer(connectionString));

    services.AddScoped<IInvoiceRepository, InvoiceRepository>();
    services.AddScoped<IPointOfSaleRepository, PointOfSaleRepository>();

    services.AddScoped<IUnitOfWork, UnitOfWork>();

    return services;
  }
}