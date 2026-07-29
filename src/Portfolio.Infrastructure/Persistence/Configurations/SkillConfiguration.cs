using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Persistence.Configurations;

public class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        builder.HasKey(skill => skill.Id);

        builder.Property(skill => skill.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(skill => skill.Name)
            .IsUnique();
    }
}