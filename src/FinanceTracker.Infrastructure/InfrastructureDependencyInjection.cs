using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Domain.Interfaces;
using FinanceTracker.Infrastructure.Files;
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

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();

        // Services
        services.AddScoped<IAuthInfrastructureService, AuthInfrastructureService>();
        services.AddScoped<IFileGenerator, MiniExcelFileGenerator>();

        return services;
    }
}
