using Microsoft.AspNetCore.Mvc;
using Portfolio.Web.ViewModels.Projects;
using Portfolio.Application.Services;

namespace Portfolio.Web.Controllers;

public class ProjectsController : Controller
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet("/projects")]
    public IActionResult Index()
    {

        var projects = _projectService.GetAll();

        var viewModels = projects.Select(project =>
            new ProjectCardViewModel
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                RepositoryUrl = project.RepositoryUrl,
                DemoUrl = project.DemoUrl,
                Technologies = []
            });

        return View(viewModels);
    }

    [HttpGet("/projects/{id:int}")]
    public IActionResult Details(int id)
    {
        var project = _projectService.GetById(id);

        if (project is null)
        {
            return NotFound();
        }

        var viewModel = new ProjectCardViewModel
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            RepositoryUrl = project.RepositoryUrl,
            DemoUrl = project.DemoUrl,
            Technologies = []
        };

        return View(viewModel);
    }

}