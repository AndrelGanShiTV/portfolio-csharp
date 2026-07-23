using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Web.Models;
using Portfolio.Web.ViewModels.Projects;

namespace Portfolio.Web.Controllers;

public class ProjectsController : Controller
{

    [HttpGet("/projects")]
    public IActionResult Index()
    {
        var projects = new List<ProjectCardViewModel>
        {
            new()
            {
                Id = 1,
                Name = "Sistema de Gestión de Proyectos Sociales",
                Description =
                    "Aplicación para administrar proyectos, actividades y participantes.",

                RepositoryUrl = "https://github.com/",
                Technologies = ["C#", "ASP.NET Core", "SQL Server"]
            },

            new()
            {
                Id = 2,
                Name = "API de Seguimiento",
                Description =
                    "API REST para consultar y actualizar información de seguimiento.",

                RepositoryUrl = "https://github.com/",
                Technologies = ["C#", "ASP.NET Core", "REST"]
            },

            new()
            {
                Id = 3,
                Name = "Dashboard de Indicadores",
                Description =
                    "Panel para visualizar indicadores y resultados de proyectos.",

                Technologies = ["C#", "Razor", "Bootstrap"]
            }
        };
        return View(projects);
    }

    [HttpGet("/projects/{id:int}")]
    public IActionResult Details(int id)
    {
        var project = new ProjectCardViewModel
        {
            Id = id,
            Name = "Sistema de Gestión de Proyectos Sociales",
            Description =
                "Aplicación para administrar proyectos, actividades y participantes.",
            RepositoryUrl = "https://github.com/",
            Technologies = ["C#", "ASP.NET Core", "SQL Server"]
        };

        return View(project);
    }

}