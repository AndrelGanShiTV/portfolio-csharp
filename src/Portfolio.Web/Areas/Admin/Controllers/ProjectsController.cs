using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Services;
using Portfolio.Domain.Entities;
using Portfolio.Web.ViewModels.Projects;
using Microsoft.AspNetCore.Authorization;

namespace Portfolio.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProjectsController : Controller
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

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

    [HttpGet("/admin/projects/create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost("/admin/projects/create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
    CreateProjectViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var project = new Project
        {
            Name = model.Name,
            Description = model.Description,
            RepositoryUrl = model.RepositoryUrl,
            DemoUrl = model.DemoUrl,
            IsPublished = model.IsPublished
        };

        await _projectService.CreateAsync(project);

        return RedirectToAction(
            actionName: nameof(Index),
            controllerName: "Projects",
            routeValues: new { area = "Admin" });
    }

    [HttpGet("/admin/projects/edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var project = await _projectService.GetByIdAsync(id);

        if (project is null)
        {
            return NotFound();
        }

        var model = new EditProjectViewModel
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            RepositoryUrl = project.RepositoryUrl,
            DemoUrl = project.DemoUrl,
            IsPublished = project.IsPublished
        };

        return View(model);
    }

    [HttpPost("/admin/projects/edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
    int id,
    EditProjectViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var project = new Project
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            RepositoryUrl = model.RepositoryUrl,
            DemoUrl = model.DemoUrl,
            IsPublished = model.IsPublished
        };

        var updated = await _projectService.UpdateAsync(project);

        if (!updated)
        {
            return NotFound();
        }

        return RedirectToAction(
            actionName: nameof(Index),
            controllerName: "Projects",
            routeValues: new { area = "Admin" });
    }

    [HttpGet("/admin/projects/delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var project = await _projectService.GetByIdAsync(id);

        if (project is null)
        {
            return NotFound();
        }

        var model = new ProjectCardViewModel
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

        return View(model);
    }

    [HttpPost("/admin/projects/delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var deleted = await _projectService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return RedirectToAction(
            actionName: nameof(Index),
            controllerName: "Projects",
            routeValues: new { area = "Admin" });
    }
}