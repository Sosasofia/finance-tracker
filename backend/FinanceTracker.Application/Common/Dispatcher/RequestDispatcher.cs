using System.Reflection;
using FinanceTracker.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceTracker.Application.Common.Dispatcher;

public sealed class RequestDispatcher(IServiceProvider serviceProvider) : IRequestDispatcher
{
    private static readonly MethodInfo DispatchMethod = typeof(RequestDispatcher)
        .GetMethod(nameof(Dispatch), BindingFlags.Instance | BindingFlags.NonPublic)!;

    public Task<TResponse> Send<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var method = DispatchMethod.MakeGenericMethod(request.GetType(), typeof(TResponse));
        return (Task<TResponse>)method.Invoke(this, [request, cancellationToken])!;
    }

    private Task<TResponse> Dispatch<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken)
        where TRequest : IRequest<TResponse>
    {
        var handler = serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
        var decorators = serviceProvider
            .GetServices<IRequestDecorator<TRequest, TResponse>>()
            .Reverse()
            .ToArray();

        RequestHandlerDelegate<TResponse> next = () => handler.Handle(request, cancellationToken);

        foreach (var decorator in decorators)
        {
            var current = next;
            next = () => decorator.Handle(request, current, cancellationToken);
        }

        return next();
    }
}
