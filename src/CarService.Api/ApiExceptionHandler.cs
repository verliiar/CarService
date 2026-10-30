using CarService.Domain;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CarService.Api;

public sealed partial class ApiExceptionHandler(IProblemDetailsService problems, ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is OperationCanceledException && cancellationToken.IsCancellationRequested)
        {
            return false;
        }

        var status = exception is BusinessException business ? business.Kind switch
        {
            FailureKind.Unauthorized => 401,
            FailureKind.Validation => 400,
            FailureKind.NotFound => 404,
            FailureKind.Forbidden => 403,
            FailureKind.Conflict => 409,
            _ => 500
        } : 500;
        if (status == 500)
        {
            LogUnhandledApiError(logger, exception);
        }

        httpContext.Response.StatusCode = status;
        return await problems.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Status = status,
                Title = status switch
                {
                    401 => "Unauthorized",
                    400 => "Validation failed",
                    403 => "Forbidden",
                    404 => "Not found",
                    409 => "Conflict",
                    _ => "Internal server error"
                },
                Detail = status == 500 ? "An unexpected error occurred." : exception.Message
            }
        });
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Unhandled API error")]
    private static partial void LogUnhandledApiError(ILogger logger, Exception exception);
}
