using Azure;
using FinanceTracker.Application.Common.Exceptions;
using FinanceTracker.Domain.Exceptions;
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

            case DomainException domainEx:
                statusCode = StatusCodes.Status400BadRequest;
                title = "Business Rule Violation";
                detail = domainEx.Message;
                break;

            case UnauthorizedAccessException:
                statusCode = StatusCodes.Status401Unauthorized;
                title = "Unauthorized";
                detail = "You must be authenticated to perform this action.";
                break;

            case ForbiddenAccessException:
                statusCode = StatusCodes.Status403Forbidden;
                title = "Forbidden";
                detail = "You do not have permission to access this resource.";
                break;

            case NotFoundException notFoundEx:
                statusCode = StatusCodes.Status404NotFound;
                title = "Resource Not Found";
                detail = notFoundEx.Message;
                break;

            case RequestFailedException azureEx:
                statusCode = StatusCodes.Status502BadGateway;
                title = "External Service Unavailable";
                detail = "The document scanning service is currently unavailable. Please try again later.";

                _logger.LogWarning("Azure Cognitive Service failed with status {Status}: {ErrorCode}", azureEx.Status, azureEx.ErrorCode);
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
