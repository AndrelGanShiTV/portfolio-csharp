namespace Portfolio.Domain.Entities;

public class Project
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? RepositoryUrl { get; set; }

    public string? DemoUrl { get; set; }

    public bool IsPublished { get; set; }

    public ICollection<ProjectSkill> ProjectSkills { get; set; }
    = new List<ProjectSkill>();
}