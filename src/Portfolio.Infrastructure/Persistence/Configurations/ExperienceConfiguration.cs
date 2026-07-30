using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Persistence.Configurations;

public class ExperienceConfiguration
    : IEntityTypeConfiguration<Experience>
{
    public void Configure(
        EntityTypeBuilder<Experience> builder)
    {
        builder.ToTable("Experiences");

        builder.HasKey(experience => experience.Id);

        builder.Property(experience => experience.Company)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(experience => experience.Position)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(experience => experience.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(experience => experience.StartDate)
            .IsRequired();

        builder.Property(experience => experience.EndDate);

        builder.Property(experience => experience.IsCurrent)
            .IsRequired();

        builder.Property(experience => experience.DisplayOrder)
            .IsRequired();
    }
}