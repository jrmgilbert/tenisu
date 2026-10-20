using Microsoft.AspNetCore.Diagnostics;

using Tenisu.Application.Common;
using Tenisu.Core.Common;

namespace Tenisu.Api;

internal sealed class TenisuExceptionHandler(IProblemDetailsService problemDetails) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        int? status = exception switch
        {
            DomainException => StatusCodes.Status400BadRequest,
            ConflictException => StatusCodes.Status409Conflict,
            _ => null
        };

        if (status is null)
            return false;

        httpContext.Response.StatusCode = status.Value;

        return await problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = { Status = status, Detail = exception.Message }
        });
    }
}