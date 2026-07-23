namespace Portfolio.Web.ViewModels.Projects;

public class ProjectCardViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? RepositoryUrl { get; set; }

    public string? DemoUrl { get; set; }

    public List<string> Technologies { get; set; } = [];
}