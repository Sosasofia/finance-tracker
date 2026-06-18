using System.Reflection;
using FinanceTracker.Application.Common.Behaviors;
using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceTracker.Application;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddValidatorsFromAssembly(assembly);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddScoped<IReceiptMappingService, ReceiptMappingService>();

        return services;
    }
}
