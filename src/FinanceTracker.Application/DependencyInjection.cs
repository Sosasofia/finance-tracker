using System.Reflection;
using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Application.Interfaces.Services;
using FinanceTracker.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceTracker.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IAuthApplicationService, AuthApplicationService>();
        services.AddScoped<IPaymentMethodService, PaymentMethodService>();
        services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());

        return services;
    }
}