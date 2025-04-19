using Microsoft.EntityFrameworkCore;
using FinanceTracker.Server.Models;

var builder = WebApplication.CreateBuilder(args);



var app = builder.Build();

app.MapGet("/", () => "¡API corriendo desde Railway!");


app.Run();
