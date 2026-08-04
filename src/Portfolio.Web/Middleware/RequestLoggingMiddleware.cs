using System.Diagnostics;

namespace Portfolio.Web.Middleware;

public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (ShouldSkipLogging(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            var statusCode = context.Response.StatusCode;

            if (statusCode is StatusCodes.Status401Unauthorized
                or StatusCodes.Status403Forbidden)
            {
                _logger.LogWarning(
                    "Security response completed. Method={Method} Path={Path} StatusCode={StatusCode} ElapsedMilliseconds={ElapsedMilliseconds} ClientIp={ClientIp} CorrelationId={CorrelationId}",
                    context.Request.Method,
                    context.Request.Path.Value,
                    statusCode,
                    stopwatch.Elapsed.TotalMilliseconds,
                    context.Connection.RemoteIpAddress?.ToString(),
                    context.TraceIdentifier);
            }
            else
            {
                _logger.LogInformation(
                    "HTTP request completed. Method={Method} Path={Path} StatusCode={StatusCode} ElapsedMilliseconds={ElapsedMilliseconds} ClientIp={ClientIp} CorrelationId={CorrelationId}",
                    context.Request.Method,
                    context.Request.Path.Value,
                    statusCode,
                    stopwatch.Elapsed.TotalMilliseconds,
                    context.Connection.RemoteIpAddress?.ToString(),
                    context.TraceIdentifier);
            }
        }
    }

    private static bool ShouldSkipLogging(PathString path)
    {
        return path.StartsWithSegments("/css")
            || path.StartsWithSegments("/js")
            || path.StartsWithSegments("/lib")
            || path.StartsWithSegments("/images")
            || path.StartsWithSegments("/favicon.ico");
    }
}