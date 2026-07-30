using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Services;
using Portfolio.Web.ViewModels.Contact;
using Portfolio.Web.ViewModels.Common;

namespace Portfolio.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ContactMessagesController : Controller
{
    private readonly IContactMessageService _contactMessageService;

    public ContactMessagesController(
        IContactMessageService contactMessageService)
    {
        _contactMessageService = contactMessageService;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        const int pageSize = 10;

        var result = await _contactMessageService.GetPagedAsync(
            page,
            pageSize);

        var items = result.Items
            .Select(message =>
                new ContactMessageAdminViewModel
                {
                    Id = message.Id,
                    Name = message.Name,
                    Email = message.Email,
                    Subject = message.Subject,
                    Message = message.Message,
                    CreatedAtUtc = message.CreatedAtUtc,
                    IsRead = message.IsRead
                })
            .ToList();

        var model =
            new PagedResultViewModel<ContactMessageAdminViewModel>
            {
                Items = items,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                TotalItems = result.TotalItems
            };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var message = await _contactMessageService.GetByIdAsync(id);

        if (message is null)
        {
            return NotFound();
        }

        if (!message.IsRead)
        {
            await _contactMessageService.MarkAsReadAsync(id);
            message.IsRead = true;
        }

        var model = new ContactMessageAdminViewModel
        {
            Id = message.Id,
            Name = message.Name,
            Email = message.Email,
            Subject = message.Subject,
            Message = message.Message,
            CreatedAtUtc = message.CreatedAtUtc,
            IsRead = message.IsRead
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var message = await _contactMessageService.GetByIdAsync(id);

        if (message is null)
        {
            return NotFound();
        }

        var model = new ContactMessageAdminViewModel
        {
            Id = message.Id,
            Name = message.Name,
            Email = message.Email,
            Subject = message.Subject,
            Message = message.Message,
            CreatedAtUtc = message.CreatedAtUtc,
            IsRead = message.IsRead
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var deleted = await _contactMessageService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }
}