using Microsoft.EntityFrameworkCore;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProjectSkill>()
            .HasKey(projectSkill => new
            {
                projectSkill.ProjectId,
                projectSkill.SkillId
            });

        modelBuilder.Entity<ProjectSkill>()
            .HasOne(projectSkill => projectSkill.Project)
            .WithMany(project => project.ProjectSkills)
            .HasForeignKey(projectSkill => projectSkill.ProjectId);

        modelBuilder.Entity<ProjectSkill>()
            .HasOne(projectSkill => projectSkill.Skill)
            .WithMany(skill => skill.ProjectSkills)
            .HasForeignKey(projectSkill => projectSkill.SkillId);
    }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<ProjectSkill> ProjectSkills => Set<ProjectSkill>();
}