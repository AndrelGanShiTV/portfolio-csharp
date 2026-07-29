namespace Portfolio.Domain.Entities;

public class Skill
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<ProjectSkill> ProjectSkills { get; set; }
        = new List<ProjectSkill>();
}