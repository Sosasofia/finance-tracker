using System.Reflection;
using FinanceTracker.Application.Common.Behaviors;
using FinanceTracker.Application.Common.Dispatcher;
using FinanceTracker.Application.Common.Interfaces;
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

        services.AddScoped<IRequestDispatcher, RequestDispatcher>();
        services.AddScoped(typeof(IRequestDecorator<,>), typeof(ValidationBehavior<,>));
        services.AddScoped(typeof(IRequestDecorator<,>), typeof(OpenTelemetryBehavior<,>));

        foreach (var handlerType in assembly.GetTypes()
                     .Where(type => !type.IsAbstract && !type.IsInterface)
                     .SelectMany(type => type.GetInterfaces()
                         .Where(interfaceType => interfaceType.IsGenericType &&
                             interfaceType.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
                         .Select(interfaceType => new { interfaceType, implementationType = type })))
        {
            services.AddScoped(handlerType.interfaceType, handlerType.implementationType);
        }

        services.AddScoped<IReceiptMappingService, ReceiptMappingService>();

        return services;
    }
}
