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
                case UnauthorizedAccessException unauthorizedEx:
                    statusCode = StatusCodes.Status401Unauthorized;
                    title = "Unauthorized";
                    detail = unauthorizedEx.Message;
                    break;
                case InvalidOperationException invalidOperation:
                    statusCode = StatusCodes.Status409Conflict;
                    title = "Conflict";
                    detail = invalidOperation.Message;
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
