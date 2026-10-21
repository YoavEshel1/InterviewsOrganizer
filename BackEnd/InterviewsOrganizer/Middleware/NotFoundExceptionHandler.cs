using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace InterviewsOrganizer.Middleware
{
    /// <summary>
    /// Maps domain "not found" exceptions to a 404 ProblemDetails response.
    /// Registered via <c>AddExceptionHandler</c> / <c>UseExceptionHandler</c> in Program.cs.
    /// </summary>
    public class NotFoundExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is not KeyNotFoundException)
            {
                return false;
            }

            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;

            await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Resource not found",
                Detail = exception.Message
            }, cancellationToken);

            return true;
        }
    }
}
