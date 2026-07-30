using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Services;
using Portfolio.Domain.Entities;
using Portfolio.Web.ViewModels.Experiences;

namespace Portfolio.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ExperiencesController : Controller
{
    private readonly IExperienceService _experienceService;

    public ExperiencesController(
        IExperienceService experienceService)
    {
        _experienceService = experienceService;
    }

    public async Task<IActionResult> Index()
    {
        var experiences = await _experienceService.GetAllAsync();

        var models = experiences.Select(experience =>
            new ExperienceViewModel
            {
                Id = experience.Id,
                Company = experience.Company,
                Position = experience.Position,
                Description = experience.Description,
                StartDate = experience.StartDate,
                EndDate = experience.EndDate,
                IsCurrent = experience.IsCurrent,
                DisplayOrder = experience.DisplayOrder
            });

        return View(models);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new ExperienceViewModel
        {
            StartDate = DateOnly.FromDateTime(DateTime.Today)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        ExperienceViewModel model)
    {
        ValidateDates(model);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var experience = MapToEntity(model);

        await _experienceService.CreateAsync(experience);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var experience = await _experienceService.GetByIdAsync(id);

        if (experience is null)
        {
            return NotFound();
        }

        var model = MapToViewModel(experience);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        ExperienceViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        ValidateDates(model);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var updated = await _experienceService.UpdateAsync(
            MapToEntity(model));

        if (!updated)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var experience = await _experienceService.GetByIdAsync(id);

        if (experience is null)
        {
            return NotFound();
        }

        return View(MapToViewModel(experience));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var deleted = await _experienceService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    private void ValidateDates(ExperienceViewModel model)
    {
        if (model.IsCurrent)
        {
            model.EndDate = null;
        }

        if (!model.IsCurrent && model.EndDate is null)
        {
            ModelState.AddModelError(
                nameof(model.EndDate),
                "La fecha de finalización es obligatoria.");
        }

        if (model.EndDate.HasValue &&
            model.EndDate.Value < model.StartDate)
        {
            ModelState.AddModelError(
                nameof(model.EndDate),
                "La fecha de finalización no puede ser anterior al inicio.");
        }
    }

    private static Experience MapToEntity(
        ExperienceViewModel model)
    {
        return new Experience
        {
            Id = model.Id,
            Company = model.Company.Trim(),
            Position = model.Position.Trim(),
            Description = model.Description.Trim(),
            StartDate = model.StartDate,
            EndDate = model.IsCurrent ? null : model.EndDate,
            IsCurrent = model.IsCurrent,
            DisplayOrder = model.DisplayOrder
        };
    }

    private static ExperienceViewModel MapToViewModel(
        Experience experience)
    {
        return new ExperienceViewModel
        {
            Id = experience.Id,
            Company = experience.Company,
            Position = experience.Position,
            Description = experience.Description,
            StartDate = experience.StartDate,
            EndDate = experience.EndDate,
            IsCurrent = experience.IsCurrent,
            DisplayOrder = experience.DisplayOrder
        };
    }
}