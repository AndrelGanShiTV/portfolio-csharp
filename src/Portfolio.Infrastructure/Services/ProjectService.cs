using Portfolio.Application.Services;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Services;

public class ProjectService : IProjectService
{
    private readonly List<Project> _projects =
    [
        new Project
        {
            Id = 1,
            Name = "Sistema de Gestión de Proyectos Sociales",
            Description =
                "Aplicación para administrar proyectos, actividades y participantes.",
            RepositoryUrl = "https://github.com/",
            IsPublished = true
        },

        new Project
        {
            Id = 2,
            Name = "API de Seguimiento",
            Description =
                "API REST para consultar y actualizar información de seguimiento.",
            RepositoryUrl = "https://github.com/",
            IsPublished = true
        },

        new Project
        {
            Id = 3,
            Name = "Dashboard de Indicadores",
            Description =
                "Panel para visualizar indicadores y resultados de proyectos.",
            IsPublished = true
        }
    ];

    public IEnumerable<Project> GetAll()
    {
        return _projects;
    }

    public Project? GetById(int id)
    {
        return _projects.FirstOrDefault(project => project.Id == id);
    }
}