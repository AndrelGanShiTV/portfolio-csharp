using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Services;
using Portfolio.Domain.Entities;
using Portfolio.Web.ViewModels.Contact;
using Microsoft.AspNetCore.RateLimiting;

namespace Portfolio.Web.Controllers;

public class ContactController : Controller
{
    private readonly IContactMessageService _contactMessageService;

    public ContactController(
        IContactMessageService contactMessageService)
    {
        _contactMessageService = contactMessageService;
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

        TempData["ContactSuccess"] =
            "Tu mensaje fue enviado correctamente.";

        return RedirectToAction(nameof(Index));
    }
}