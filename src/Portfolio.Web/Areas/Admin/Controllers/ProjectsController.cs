using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Services;
using Portfolio.Domain.Entities;
using Portfolio.Web.ViewModels.Projects;
using Portfolio.Web.ViewModels.Skills;
using Microsoft.AspNetCore.Authorization;

namespace Portfolio.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProjectsController : Controller
{
    private readonly IProjectService _projectService;
    private readonly ISkillService _skillService;
    public ProjectsController(
    IProjectService projectService,
    ISkillService skillService)
    {
        _projectService = projectService;
        _skillService = skillService;
    }

    public async Task<IActionResult> Index()
    {
        var projects = await _projectService.GetAllForAdminAsync();

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
                    .ToList(),
                IsPublished = project.IsPublished
            });

        return View(viewModels);
    }

    [HttpGet("/admin/projects/create")]
    public async Task<IActionResult> Create()
    {

        var model = new CreateProjectViewModel();

        await LoadAvailableSkillsAsync(model);

        return View(model);
    }

    [HttpPost("/admin/projects/create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
    CreateProjectViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadAvailableSkillsAsync(model);
            return View(model);
        }

        var project = new Project
        {
            Name = model.Name,
            Description = model.Description,
            RepositoryUrl = model.RepositoryUrl,
            DemoUrl = model.DemoUrl,
            IsPublished = model.IsPublished,

            ProjectSkills = model.SelectedSkillIds
                .Select(skillId => new ProjectSkill
                {
                    SkillId = skillId
                })
                .ToList()
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
        var project = await _projectService.GetByIdForAdminAsync(id);

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
            IsPublished = project.IsPublished,
            SelectedSkillIds = project.ProjectSkills
                .Select(ps => ps.SkillId)
                .ToList(),
        };

        await LoadAvailableSkillsAsync(model);


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
            await LoadAvailableSkillsAsync(model);
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

        var updated = await _projectService.UpdateAsync(project, model.SelectedSkillIds);

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
        var project = await _projectService.GetByIdForAdminAsync(id);

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

    // This method loads the available skills from the skill service and populates the AvailableSkills property of the CreateProjectViewModel.
    private async Task LoadAvailableSkillsAsync(
    CreateProjectViewModel model)
    {
        var skills = await _skillService.GetAllAsync();

        model.AvailableSkills = skills
            .Select(skill => new SkillOptionViewModel
            {
                Id = skill.Id,
                Name = skill.Name
            })
            .ToList();
    }

    // This method loads the available skills from the skill service and populates the AvailableSkills property of the EditProjectViewModel.
    private async Task LoadAvailableSkillsAsync(
    EditProjectViewModel model)
    {
        var skills = await _skillService.GetAllAsync();

        model.AvailableSkills = skills
            .Select(skill => new SkillOptionViewModel
            {
                Id = skill.Id,
                Name = skill.Name
            })
            .ToList();
    }
}