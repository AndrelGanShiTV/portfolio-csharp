using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Services;
using Portfolio.Domain.Entities;
using Portfolio.Web.ViewModels.Contact;

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
        return View(new ContactFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(
        ContactFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
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