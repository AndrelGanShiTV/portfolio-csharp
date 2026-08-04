using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Portfolio.Application.Abstractions;
using Portfolio.Domain.Entities;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Infrastructure.Auditing;

public sealed class AuditLogger : IAuditLogger
{
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditLogger(
        AppDbContext dbContext,
        IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task WriteAsync(
        string action,
        bool succeeded,
        string? resourceType = null,
        string? resourceId = null,
        string? details = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(action);

        var httpContext = _httpContextAccessor.HttpContext;
        var currentUser = httpContext?.User;

        var auditLog = new AuditLog
        {
            OccurredAtUtc = DateTimeOffset.UtcNow,

            UserId = currentUser?.FindFirstValue(
                ClaimTypes.NameIdentifier),

            UserEmail = currentUser?.FindFirstValue(
                ClaimTypes.Email)
                ?? currentUser?.Identity?.Name,

            Action = action,
            ResourceType = resourceType,
            ResourceId = resourceId,

            IpAddress = GetClientIpAddress(httpContext),

            Succeeded = succeeded,
            Details = NormalizeDetails(details)
        };

        _dbContext.AuditLogs.Add(auditLog);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static string? NormalizeDetails(string? details)
    {
        if (string.IsNullOrWhiteSpace(details))
        {
            return null;
        }

        var normalizedDetails = details.Trim();

        return normalizedDetails.Length <= 1000
            ? normalizedDetails
            : normalizedDetails[..1000];
    }

    private static string? GetClientIpAddress(HttpContext? httpContext)
    {
        if (httpContext is null)
        {
            return null;
        }

        return httpContext.Connection.RemoteIpAddress?.ToString();
    }
}