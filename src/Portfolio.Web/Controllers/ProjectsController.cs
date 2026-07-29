using Microsoft.AspNetCore.Mvc;
using Portfolio.Web.ViewModels.Projects;
using Portfolio.Application.Services;
using Portfolio.Domain.Entities;

namespace Portfolio.Web.Controllers;

public class ProjectsController : Controller
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet("/projects")]
    public async Task<IActionResult> Index()
    {

        var projects = await _projectService.GetAllAsync();

        var viewModels = projects.Select(project =>
            new ProjectCardViewModel
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                RepositoryUrl = project.RepositoryUrl,
                DemoUrl = project.DemoUrl,
                Technologies = project.ProjectSkills
                    .Select(projectSkill => projectSkill.Skill.Name)
                    .ToList()
            });

        return View(viewModels);
    }

    [HttpGet("/projects/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var project = await _projectService.GetByIdAsync(id);

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
            Technologies = project.ProjectSkills
                .Select(projectSkill => projectSkill.Skill.Name)
                .ToList()
        };

        return View(viewModel);
    }
}