using System.ComponentModel.DataAnnotations;
using Portfolio.Web.ViewModels.Skills;

namespace Portfolio.Web.ViewModels.Projects;

public class CreateProjectViewModel
{
    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Url]
    [StringLength(500)]
    public string? RepositoryUrl { get; set; }

    [Url]
    [StringLength(500)]
    public string? DemoUrl { get; set; }

    public bool IsPublished { get; set; }

    public List<int> SelectedSkillIds { get; set; } = [];

    public List<SkillOptionViewModel> AvailableSkills { get; set; } = [];
}