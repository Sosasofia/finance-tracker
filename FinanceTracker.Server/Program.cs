using Microsoft.EntityFrameworkCore;
using FinanceTracker.Server.Models;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration["ConnectionStrings:FinanceDB"];
Console.WriteLine($"Connection string: {connectionString}");

if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception("Connection string 'FinanceDB' not found.");
}

builder.Services.AddDbContext<Context>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

var app = builder.Build();

app.MapGet("/", () => "¡API corriendo desde Railway!");

try
{
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine("Error al ejecutar la app:");
    Console.WriteLine(ex.ToString());
}
