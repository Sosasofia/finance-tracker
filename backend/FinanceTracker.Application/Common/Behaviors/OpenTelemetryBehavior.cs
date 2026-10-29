using System.Diagnostics;
using FinanceTracker.Application.Common.Interfaces;

namespace FinanceTracker.Application.Common.Behaviors;

public class OpenTelemetryBehavior<TRequest, TResponse> : IRequestDecorator<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private static readonly ActivitySource _activitySource = new ActivitySource("FinanceTracker.Application");

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        using var activity = _activitySource.StartActivity($"Application request {requestName}");
        activity?.SetTag("application.request_name", requestName);

        try
        {
            var response = await next();
            activity?.SetStatus(ActivityStatusCode.Ok);
            return response;
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }
}
