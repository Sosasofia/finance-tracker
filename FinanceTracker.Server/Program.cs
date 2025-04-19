using Microsoft.EntityFrameworkCore;
using FinanceTracker.Server.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration["ConnectionStrings:FinanceDB"];
Console.WriteLine($"Connection string: {connectionString}");


builder.Services.AddDbContext<Context>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

var app = builder.Build();

app.MapGet("/", () => "¡API corriendo desde Railway!");


app.Run();
