namespace Portfolio.Web.Middleware;

public sealed class CorrelationIdMiddleware
{
    private const string CorrelationIdHeader = "X-Correlation-ID";

    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetOrCreateCorrelationId(context);

        context.TraceIdentifier = correlationId;

        context.Response.OnStarting(() =>
        {
            context.Response.Headers[CorrelationIdHeader] = correlationId;
            return Task.CompletedTask;
        });

        await _next(context);
    }

    private static string GetOrCreateCorrelationId(HttpContext context)
    {
        var incomingCorrelationId =
            context.Request.Headers[CorrelationIdHeader].FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(incomingCorrelationId)
            && incomingCorrelationId.Length <= 100)
        {
            return incomingCorrelationId;
        }

        return Guid.NewGuid().ToString("N");
    }
}