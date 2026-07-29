using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Persistence.Configurations;

public class ProjectSkillConfiguration
    : IEntityTypeConfiguration<ProjectSkill>
{
    public void Configure(
        EntityTypeBuilder<ProjectSkill> builder)
    {
        builder.HasKey(projectSkill => new
        {
            projectSkill.ProjectId,
            projectSkill.SkillId
        });

        builder
            .HasOne(projectSkill => projectSkill.Project)
            .WithMany(project => project.ProjectSkills)
            .HasForeignKey(projectSkill => projectSkill.ProjectId);

        builder
            .HasOne(projectSkill => projectSkill.Skill)
            .WithMany(skill => skill.ProjectSkills)
            .HasForeignKey(projectSkill => projectSkill.SkillId);
    }
}