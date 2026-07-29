using Microsoft.EntityFrameworkCore;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Projects.AnyAsync())
        {
            return;
        }

        var projects = new List<Project>
        {
            new()
            {
                Name = "Sistema de Gestión de Proyectos Sociales",
                Description =
                    "Aplicación para administrar proyectos, actividades y participantes.",
                RepositoryUrl = "https://github.com/",
                IsPublished = true
            },

            new()
            {
                Name = "API de Seguimiento",
                Description =
                    "API REST para consultar y actualizar información de seguimiento.",
                RepositoryUrl = "https://github.com/",
                IsPublished = true
            },

            new()
            {
                Name = "Dashboard de Indicadores",
                Description =
                    "Panel para visualizar indicadores y resultados de proyectos.",
                IsPublished = true
            }
        };

        await context.Projects.AddRangeAsync(projects);

        await context.SaveChangesAsync();
    }
}