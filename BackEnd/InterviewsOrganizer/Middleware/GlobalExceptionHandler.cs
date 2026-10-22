using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace InterviewsOrganizer.Middleware
{
    /// <summary>
    /// Catch-all handler for any exception not handled by a more specific <see cref="IExceptionHandler"/>
    /// (e.g. <see cref="NotFoundExceptionHandler"/>). Logs the full exception server-side and returns a
    /// generic, non-leaking 500 ProblemDetails response to the client.
    /// Registered via <c>AddExceptionHandler</c> / <c>UseExceptionHandler</c> in Program.cs.
    /// IMPORTANT: Must be registered AFTER other, more specific exception handlers so it only runs
    /// as a last resort (ASP.NET Core invokes handlers in registration order).
    /// </summary>
    public class GlobalExceptionHandler : IExceptionHandler
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
            var traceId = httpContext.TraceIdentifier;

            _logger.LogError(
                exception,
                "Unhandled exception occurred. TraceId: {TraceId}, Method: {HttpMethod}, Path: {Path}",
                traceId,
                httpContext.Request.Method,
                httpContext.Request.Path);

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred.",
                Detail = "An internal server error occurred. Please contact support if the problem persists.",
                Extensions = { ["traceId"] = traceId }
            }, cancellationToken);

            return true;
        }
    }
}
