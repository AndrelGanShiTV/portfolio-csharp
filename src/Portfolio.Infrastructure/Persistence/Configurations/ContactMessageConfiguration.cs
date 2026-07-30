using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Persistence.Configurations;

public class ContactMessageConfiguration
    : IEntityTypeConfiguration<ContactMessage>
{
    public void Configure(
        EntityTypeBuilder<ContactMessage> builder)
    {
        builder.ToTable("ContactMessages");

        builder.HasKey(message => message.Id);

        builder.Property(message => message.Name)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(message => message.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(message => message.Subject)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(message => message.Message)
            .IsRequired()
            .HasMaxLength(3000);

        builder.Property(message => message.CreatedAtUtc)
            .IsRequired();

        builder.Property(message => message.IsRead)
            .IsRequired();
    }
}