using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Services;
using Portfolio.Domain.Entities;
using Portfolio.Web.ViewModels.Experiences;
using Portfolio.Application.Abstractions;

namespace Portfolio.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ExperiencesController : Controller
{
    private readonly IExperienceService _experienceService;
    private readonly IAuditLogger _auditLogger;

    public ExperiencesController(
        IExperienceService experienceService,
        IAuditLogger auditLogger)
    {
        _experienceService = experienceService;
        _auditLogger = auditLogger;
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

        await _auditLogger.WriteAsync(
            action: "ExperienceCreated",
            succeeded: true,
            resourceType: "Experience",
            resourceId: experience.Id.ToString(),
            details: $"Experience ID:{experience.Id} - '{experience.Position}' ha sido creada");

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

        await _auditLogger.WriteAsync(
            action: "ExperienceUpdated",
            succeeded: true,
            resourceType: "Experience",
            resourceId: model.Id.ToString(),
            details: $"Experience ID:{model.Id} - '{model.Position}' ha sido actualizada");

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
        var experience = await _experienceService.GetByIdAsync(id);
        var deleted = await _experienceService.DeleteAsync(id);

        await _auditLogger.WriteAsync(
            action: "ExperienceDeleted",
            succeeded: true,
            resourceType: "Experience",
            resourceId: id.ToString(),
            details: $"Experience ID:{id} - '{experience?.Position}' ha sido eliminada");

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