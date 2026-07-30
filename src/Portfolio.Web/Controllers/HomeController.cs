using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Web.Models;
using Portfolio.Application.Services;
using Portfolio.Web.ViewModels.Experiences;

namespace Portfolio.Web.Controllers;

public class HomeController : Controller
{
    private readonly IExperienceService _experienceService;

    public HomeController(IExperienceService experienceService)
    {
        _experienceService = experienceService;
    }

    [HttpGet("/")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("/about")]
    public IActionResult About()
    {
        return View();
    }

    public async Task<IActionResult> Experience()
    {
        var experiences = await _experienceService.GetAllAsync();

        var models = experiences.Select(experience =>
            new ExperienceItemViewModel
            {
                Company = experience.Company,
                Position = experience.Position,
                Description = experience.Description,
                StartDate = experience.StartDate,
                EndDate = experience.EndDate,
                IsCurrent = experience.IsCurrent
            });

        return View(models);
    }
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
