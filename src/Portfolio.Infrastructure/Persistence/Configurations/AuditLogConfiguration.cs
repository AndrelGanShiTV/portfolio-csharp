using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Persistence.Configurations;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Action)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasMaxLength(450);

        builder.Property(x => x.UserEmail)
            .HasMaxLength(256);

        builder.Property(x => x.ResourceType)
            .HasMaxLength(100);

        builder.Property(x => x.ResourceId)
            .HasMaxLength(100);

        builder.Property(x => x.IpAddress)
            .HasMaxLength(64);

        builder.Property(x => x.Details)
            .HasMaxLength(1000);

        builder.HasIndex(x => x.OccurredAtUtc);
        builder.HasIndex(x => x.Action);
        builder.HasIndex(x => x.UserId);
    }
}