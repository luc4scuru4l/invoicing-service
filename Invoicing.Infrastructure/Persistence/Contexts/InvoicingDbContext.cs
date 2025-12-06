using System.Reflection;
using Invoicing.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Invoicing.Infrastructure.Persistence.Contexts;

public class InvoicingDbContext : DbContext
{
    // Constructor estándar para que la inyección de dependencias funcione
    public InvoicingDbContext(DbContextOptions<InvoicingDbContext> options) : base(options)
    {
    }

    // Tus Tablas
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceItem> InvoiceItems { get; set; }
    // public DbSet<PointOfSale> PointOfSales { get; set; } // Agregala cuando tengas la entidad
    // public DbSet<PointOfSaleCounter> PointOfSaleCounters { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // MAGIA: Esto escanea todo el proyecto buscando clases que implementen 
        // IEntityTypeConfiguration (como la que creamos en el paso 2) y las aplica.
        // Te ahorra escribir miles de líneas de configuración acá.
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}