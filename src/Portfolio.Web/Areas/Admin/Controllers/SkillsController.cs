using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Services;
using Portfolio.Domain.Entities;
using Portfolio.Web.ViewModels.Skills;

namespace Portfolio.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class SkillsController : Controller
{
    private readonly ISkillService _skillService;

    public SkillsController(ISkillService skillService)
    {
        _skillService = skillService;
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
        var deleted = await _skillService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }
}