namespace Portfolio.Domain.Entities;

public sealed class AuditLog
{
    public long Id { get; set; }

    public DateTimeOffset OccurredAtUtc { get; set; }

    public string? UserId { get; set; }

    public string? UserEmail { get; set; }

    public string Action { get; set; } = string.Empty;

    public string? ResourceType { get; set; }

    public string? ResourceId { get; set; }

    public string? IpAddress { get; set; }

    public bool Succeeded { get; set; }

    public string? Details { get; set; }
}