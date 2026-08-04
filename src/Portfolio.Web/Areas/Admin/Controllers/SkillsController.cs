using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Services;
using Portfolio.Domain.Entities;
using Portfolio.Web.ViewModels.Skills;
using Portfolio.Application.Abstractions;

namespace Portfolio.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class SkillsController : Controller
{
    private readonly ISkillService _skillService;
    private readonly IAuditLogger _auditLogger;

    public SkillsController(ISkillService skillService,
    IAuditLogger auditLogger)
    {
        _skillService = skillService;
        _auditLogger = auditLogger;
    }

    public async Task<IActionResult> Index()
    {
        var skills = await _skillService.GetAllAsync();

        var models = skills.Select(skill =>
            new SkillViewModel
            {
                Id = skill.Id,
                Name = skill.Name
            });

        return View(models);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        SkillViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var skill = new Skill
        {
            Name = model.Name.Trim()
        };

        await _skillService.CreateAsync(skill);

        await _auditLogger.WriteAsync(
            action: "SkillCreated",
            succeeded: true,
            resourceType: "Skill",
            resourceId: skill.Id.ToString(),
            details: $"Skill '{skill.Name}' ha sido creada con ID {skill.Id}.");

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var skill = await _skillService.GetByIdAsync(id);

        if (skill is null)
        {
            return NotFound();
        }

        var model = new SkillViewModel
        {
            Id = skill.Id,
            Name = skill.Name
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
    int id,
    SkillViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var skill = new Skill
        {
            Id = model.Id,
            Name = model.Name.Trim()
        };

        var updated = await _skillService.UpdateAsync(skill);

        await _auditLogger.WriteAsync(
            action: "SkillUpdated",
            succeeded: true,
            resourceType: "Skill",
            resourceId: skill.Id.ToString(),
            details: $"Skill '{skill.Name}' ha sido actualizada con ID {skill.Id}.");

        if (!updated)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var skill = await _skillService.GetByIdAsync(id);

        if (skill is null)
        {
            return NotFound();
        }

        var model = new SkillViewModel
        {
            Id = skill.Id,
            Name = skill.Name
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var skill = await _skillService.GetByIdAsync(id);
        var deleted = await _skillService.DeleteAsync(id);

        await _auditLogger.WriteAsync(
            action: "SkillDeleted",
            succeeded: true,
            resourceType: "Skill",
            resourceId: id.ToString(),
            details: $"Skill '{skill?.Name}' ha sido eliminada con ID {id}.");

        if (!deleted)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }
}