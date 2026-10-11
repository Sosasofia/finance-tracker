using Azure;
using FinanceTracker.Application.Common.Exceptions;
using FinanceTracker.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Server.Middleware;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(
            exception,
            "Unhandled exception occurred: {Message}",
            exception.Message);

        var statusCode = StatusCodes.Status500InternalServerError;
        var title = "Internal Server Error";
        var detail = "An unexpected error occurred. Please try again later.";
        IDictionary<string, string[]>? errors = null;

        switch (exception)
        {
            case ValidationException validationException:
                statusCode = StatusCodes.Status400BadRequest;
                title = "Validation Error";
                detail = "One or more validation failures have occurred.";
                errors = validationException.Errors
                    .GroupBy(error => error.PropertyName)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Select(error => error.ErrorMessage).ToArray());
                break;

            case DomainException domainException:
                statusCode = StatusCodes.Status400BadRequest;
                title = "Business Rule Violation";
                detail = domainException.Message;
                break;

            case ForbiddenAccessException:
                statusCode = StatusCodes.Status403Forbidden;
                title = "Forbidden";
                detail = "You do not have permission to access this resource.";
                break;

            case UnauthorizedAccessException ex:
                statusCode = StatusCodes.Status401Unauthorized;
                title = "Unauthorized";
                detail = ex.Message;
                break;

            case NotFoundException notFoundException:
                statusCode = StatusCodes.Status404NotFound;
                title = "Resource Not Found";
                detail = notFoundException.Message;
                break;

            case ArgumentOutOfRangeException argumentException:
                statusCode = StatusCodes.Status400BadRequest;
                title = "Invalid Argument";
                detail = argumentException.Message;
                break;

            case RequestFailedException azureException:
                statusCode = StatusCodes.Status502BadGateway;
                title = "External Service Unavailable";
                detail = "The document scanning service is currently unavailable. Please try again later.";

                _logger.LogWarning(
                    "Azure Cognitive Service failed with status {Status}: {ErrorCode}",
                    azureException.Status,
                    azureException.ErrorCode);
                break;
        }

        var problemDetails = new ProblemDetails
        {
            Title = title,
            Status = statusCode,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        if (errors is not null)
        {
            problemDetails.Extensions["errors"] = errors;
        }

        httpContext.Response.Clear();
        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }
}
