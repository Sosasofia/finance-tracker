using Azure.AI.DocumentIntelligence;
using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using FinanceTracker.Application.Common.Interfaces;
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
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace FinanceTracker.Infrastructure;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        // Database
        var connectionStringDb = configuration.GetConnectionString("FinanceDB");
        string connectionString = $"{connectionStringDb}Connection Timeout=60;";

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
        services.AddScoped<IUserPreferencesRepository, UserPreferencesRepository>();
        services.AddScoped<IReceiptRepository, ReceiptRepository>();

        // Services
        services.AddScoped<IAuthInfrastructureService, AuthInfrastructureService>();
        services.AddScoped<IFileGenerator, MiniExcelFileGenerator>();

        // Azure AI Document Intelligence
        var endpoint = configuration["AzureDesign:DocumentIntelligence:Endpoint"];
        if (!Uri.TryCreate(endpoint, UriKind.Absolute, out var documentIntelligenceEndpoint))
        {
            throw new InvalidOperationException(
                "AzureDesign:DocumentIntelligence:Endpoint must be an absolute URI.");
        }

        services.AddSingleton<TokenCredential>(_ =>
        {
            if (environment.IsDevelopment())
            {
                return new AzureCliCredential();
            }

            return new ManagedIdentityCredential(ManagedIdentityId.SystemAssigned);
        });
        
        services.AddSingleton(sp =>
            new DocumentIntelligenceClient(
                documentIntelligenceEndpoint,
                sp.GetRequiredService<TokenCredential>()));

        services.AddScoped<IReceiptScannerService, AzureReceiptScannerService>();

        // Azure Blob Storage
        services.AddOptions<BlobStorageSettings>()
            .Bind(configuration.GetSection("BlobStorage"))
            .Validate(settings => !string.IsNullOrWhiteSpace(settings.ContainerName),
                "BlobStorage:ContainerName is required.")
            .Validate(settings => settings.MaxFileSizeBytes > 0,
                "BlobStorage:MaxFileSizeBytes must be greater than zero.")
            .Validate(settings => settings.SasLifetimeMinutes is > 0 and <= 60,
                "BlobStorage:SasLifetimeMinutes must be between 1 and 60.")
            .ValidateOnStart();

        services.AddSingleton(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<BlobStorageSettings>>().Value;

            if (environment.IsDevelopment() && !string.IsNullOrWhiteSpace(settings.ConnectionString))
            {
                return new BlobServiceClient(settings.ConnectionString);
            }

            if (string.IsNullOrEmpty(settings.AccountUrl))
            {
                throw new ArgumentException("BlobStorage:AccountUrl is required.");
            }

            return new BlobServiceClient(
                new Uri(settings.AccountUrl),
                sp.GetRequiredService<TokenCredential>());
        });

        services.AddScoped<IReceiptStorageService, AzureBlobStorageService>();

        return services;
    }
}
