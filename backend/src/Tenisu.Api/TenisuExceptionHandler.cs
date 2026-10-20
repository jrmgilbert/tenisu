using Microsoft.AspNetCore.Diagnostics;

using Tenisu.Application.Common;
using Tenisu.Core.Common;

namespace Tenisu.Api;

internal sealed partial class TenisuExceptionHandler(IProblemDetailsService problemDetails, ILogger<TenisuExceptionHandler> logger)
    : IExceptionHandler
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

        LogRequestRejected(logger, httpContext.Request.Method, httpContext.Request.Path, status.Value, exception.Message);

        httpContext.Response.StatusCode = status.Value;

        return await problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = { Status = status, Detail = exception.Message }
        });
    }

    [LoggerMessage(EventId = 2001, EventName = "RequestRejected", Level = LogLevel.Warning, Message = "{Method} {Path} rejected with {StatusCode}: {Reason}")]
    private static partial void LogRequestRejected(ILogger logger, string method, string path, int statusCode, string reason);
}