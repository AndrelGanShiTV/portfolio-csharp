using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Persistence.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasKey(project => project.Id);

        builder.Property(project => project.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(project => project.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(project => project.RepositoryUrl)
            .HasMaxLength(500);

        builder.Property(project => project.DemoUrl)
            .HasMaxLength(500);
    }
}