using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Domain.Interfaces;
using FinanceTracker.Infrastructure.Persistance;
using FinanceTracker.Infrastructure.Persistance.Repositories;
using FinanceTracker.Infrastructure.Services;
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
        // Database 
        var connectionStringDB = configuration.GetConnectionString("FinanceDB");

        string connectionString = $"{connectionStringDB}" +
                          "Connection Timeout=60;";

        var serverVersion = new MySqlServerVersion(new Version(9, 3, 0));

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseMySql(connectionString, serverVersion,
                mySqlOptions =>
                {
                    mySqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 10,
                        maxRetryDelay: TimeSpan.FromSeconds(60),
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

        return services;
    }
}
