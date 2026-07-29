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

        var csharp = new Skill { Name = "C#" };
        var aspNet = new Skill { Name = "ASP.NET Core" };
        var sqlite = new Skill { Name = "SQLite" };
        var rest = new Skill { Name = "REST" };

        var project1 = new Project
        {
            Name = "Sistema de Gestión de Proyectos Sociales",
            Description = "Aplicación para administrar proyectos, actividades y participantes.",
            RepositoryUrl = "https://github.com/",
            IsPublished = true,
            ProjectSkills =
            [
                new ProjectSkill { Skill = csharp },
                new ProjectSkill { Skill = aspNet },
                new ProjectSkill { Skill = sqlite }
            ]
        };

        var project2 = new Project
        {
            Name = "API de Seguimiento",
            Description = "API REST para consultar y actualizar información de seguimiento.",
            RepositoryUrl = "https://github.com/",
            IsPublished = true,
            ProjectSkills =
            [
                new ProjectSkill { Skill = csharp },
                new ProjectSkill { Skill = aspNet },
                new ProjectSkill { Skill = rest }
            ]
        };

        await context.Projects.AddRangeAsync(project1, project2);

        await context.SaveChangesAsync();
    }
}