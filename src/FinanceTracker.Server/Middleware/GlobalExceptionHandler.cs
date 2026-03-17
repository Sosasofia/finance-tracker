using FinanceTracker.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Server.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            int statusCode = StatusCodes.Status500InternalServerError;
            string title = "Internal Server Error.";
            string detail = "An unexpected error occurred. Please try again later.";

            var logger = httpContext.RequestServices.GetRequiredService<ILogger<GlobalExceptionHandler>>();
            logger?.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);

            httpContext.Response.StatusCode = statusCode;

            switch (exception)
            {
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
                        logger?.LogCritical("DATABASE CONNECTION FAILURE: Check SQL Server status.");
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

            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
