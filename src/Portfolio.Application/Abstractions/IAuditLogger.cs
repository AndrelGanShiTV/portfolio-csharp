namespace Portfolio.Application.Abstractions;

public interface IAuditLogger
{
    Task WriteAsync(
        string action,
        bool succeeded,
        string? resourceType = null,
        string? resourceId = null,
        string? details = null,
        CancellationToken cancellationToken = default);
}