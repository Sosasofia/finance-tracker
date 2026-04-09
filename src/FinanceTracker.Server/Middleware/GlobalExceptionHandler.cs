using FinanceTracker.Application.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Server.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<GlobalExceptionHandler> logger)
    {
        _problemDetailsService = problemDetailsService;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken ct)
    {
        _logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);

        int statusCode = StatusCodes.Status500InternalServerError;
        string title = "Internal Server Error.";
        string detail = "An unexpected error occurred. Please try again later.";
        IDictionary<string, string[]>? errors = null;

        switch (exception)
        {
            case ValidationException valEx:
                statusCode = StatusCodes.Status400BadRequest;
                title = "Validation Error";
                detail = "One or more validation failures have occurred.";
                errors = valEx.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );
                break;

            case InvalidOperationException:
            case UnauthorizedAccessException:
            case ForbiddenAccessException:
            case NotFoundException:
            case DuplicateException:
                statusCode = exception switch
                {
                    NotFoundException => StatusCodes.Status404NotFound,
                    UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                    ForbiddenAccessException => StatusCodes.Status403Forbidden,
                    _ => StatusCodes.Status400BadRequest
                };
                title = "Action Failed";
                detail = exception.Message;
                break;

            default:
                if (exception.Message.Contains("SQL") || exception.Message.Contains("transient"))
                {
                    _logger.LogCritical("DATABASE CONNECTION FAILURE: Check SQL Server status.");
                }
                break;
        }

        var problemDetails = new ProblemDetails
        {
            Title = title,
            Status = statusCode,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        if (errors != null)
        {
            problemDetails.Extensions.Add("errors", errors);
        }

        httpContext.Response.StatusCode = statusCode;

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
            Exception = exception
        });
    }
}
