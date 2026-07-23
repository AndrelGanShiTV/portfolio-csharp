using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Web.Models;

namespace Portfolio.Web.Controllers;

public class ProjectsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}