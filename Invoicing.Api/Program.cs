using Invoicing.Infrastructure;
using Invoicing.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. INYECTAR EL DB CONTEXT
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
