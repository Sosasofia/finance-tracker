using System.Reflection;

using FinanceTracker.Application.Interfaces;
using FinanceTracker.Application.Services;

using Microsoft.Extensions.DependencyInjection;

namespace FinanceTracker.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationService(this IServiceCollection services)
    {
        services.AddScoped<ITransactionApplicationService, TransactionApplicationService>();
        services.AddScoped<IUserApplicationService, UserApplicationService>();
        services.AddScoped<ICategoryApplicationService, CategoryApplicationService>();
        services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());

        return services;
    }
}