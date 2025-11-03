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
            string detail = "An unexpected error occurred.";

            switch (exception)
            {
                case InvalidOperationException invalidOperation:
                    statusCode = StatusCodes.Status400BadRequest;
                    title = "Bad Request";
                    detail = invalidOperation.Message;
                    break;
                case UnauthorizedAccessException unauthorizedEx:
                    statusCode = StatusCodes.Status401Unauthorized;
                    title = "Unauthorized";
                    detail = unauthorizedEx.Message;
                    break;
                case ForbiddenAccessException forbiddenEx:
                    statusCode = StatusCodes.Status403Forbidden;
                    title = "Forbidden";
                    detail = forbiddenEx.Message;
                    break;
                case NotFoundException notFoundEx:
                    statusCode = StatusCodes.Status404NotFound;
                    title = "Not Found";
                    detail = notFoundEx.Message;
                    break;
                case ResourceInUseException:
                case DuplicateException:
                    statusCode = StatusCodes.Status409Conflict;
                    title = "Conflict";
                    detail = exception.Message;
                    break;

                default:
                    break;
            }

            var problemDetails = new ProblemDetails
            {
                Title = title,
                Status = statusCode,
                Detail = detail
            };

            httpContext.Response.StatusCode = problemDetails.Status.Value;

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
