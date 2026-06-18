using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Domain.Interfaces;
using FinanceTracker.Infrastructure.Files;
using FinanceTracker.Infrastructure.Persistence;
using FinanceTracker.Infrastructure.Repositories;
using FinanceTracker.Infrastructure.Services;
using FinanceTracker.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceTracker.Infrastructure;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Azure
        var azureEndpoint = configuration["AzureDesign:DocumentIntelligence:Endpoint"];
        var azureKey = configuration["AzureDesign:DocumentIntelligence:ApiKey"];

        // Database 
        var connectionStringDB = configuration.GetConnectionString("FinanceDB");

        string connectionString = $"{connectionStringDB}" +
                          "Connection Timeout=60;";

        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null
                );
            });
        });

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();

        // Services
        services.AddScoped<IAuthInfrastructureService, AuthInfrastructureService>();
        services.AddScoped<IFileGenerator, MiniExcelFileGenerator>();

        services.AddScoped<IReceiptScannerService>(sp =>
            new AzureReceiptScannerService(azureEndpoint, azureKey));

        return services;
    }
}
