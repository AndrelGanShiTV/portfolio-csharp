using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Services;
using Portfolio.Domain.Entities;
using Portfolio.Web.ViewModels.Contact;
using Microsoft.AspNetCore.RateLimiting;
using Portfolio.Application.Abstractions;

namespace Portfolio.Web.Controllers;

public class ContactController : Controller
{
    private readonly IContactMessageService _contactMessageService;
    private readonly IAuditLogger _auditLogger;

    public ContactController(
        IContactMessageService contactMessageService,
        IAuditLogger auditLogger)
    {
        _contactMessageService = contactMessageService;
        _auditLogger = auditLogger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new ContactFormViewModel
        {
            FormLoadedAtUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("contact-form")]
    public async Task<IActionResult> Index(
        ContactFormViewModel model)
    {
        //Hey Honey
        if (!string.IsNullOrWhiteSpace(model.Website))
        {
            TempData["ContactSuccess"] =
                "Tu mensaje fue enviado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        var submittedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var elapsedSeconds = submittedAt - model.FormLoadedAtUnix;

        if (model.FormLoadedAtUnix <= 0 || elapsedSeconds < 3 || elapsedSeconds > 3600)
        {
            TempData["ContactSuccess"] =
                "Tu mensaje fue enviado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            ModelState.Remove(nameof(model.FormLoadedAtUnix));

            model.FormLoadedAtUnix =
                DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            return View(model);
        }

        var message = new ContactMessage
        {
            Name = model.Name.Trim(),
            Email = model.Email.Trim(),
            Subject = model.Subject.Trim(),
            Message = model.Message.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
            IsRead = false
        };

        await _contactMessageService.CreateAsync(message);

        await _auditLogger.WriteAsync(
            action: "ContactMessageCreated",
            succeeded: true,
            resourceType: "ContactMessage",
            resourceId: message.Id.ToString(),
            details: $"ContactMessage ID:{message.Id} ha sido creada");

        TempData["ContactSuccess"] =
            "Tu mensaje fue enviado correctamente.";

        return RedirectToAction(nameof(Index));
    }
}