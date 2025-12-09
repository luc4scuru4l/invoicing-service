using Invoicing.Infrastructure;
using Invoicing.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
  throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no se encontró en appsettings.json");
}

builder.Services.AddDbContext<InvoicingDbContext>(options =>
{
  options.UseSqlServer(connectionString, sqlOptions =>
  {
    sqlOptions.MigrationsAssembly("Invoicing.Infrastructure");
  });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
