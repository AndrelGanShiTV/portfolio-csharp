using Microsoft.EntityFrameworkCore;
using Portfolio.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Portfolio.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(
        AppDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
        // Ensure the database is created with Skills and Projects tables
        if (!await context.Projects.AnyAsync())
        {
            var csharp = new Skill { Name = "C#" };
            var aspNet = new Skill { Name = "ASP.NET Core" };
            var sqlite = new Skill { Name = "SQLite" };
            var rest = new Skill { Name = "REST" };

            var project1 = new Project
            {
                Name = "Sistema de Gestión de Proyectos Sociales",
                Description =
                    "Aplicación para administrar proyectos, actividades y participantes.",
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
                Description =
                    "API REST para consultar y actualizar información de seguimiento.",
                IsPublished = true,
                ProjectSkills =
                [
                    new ProjectSkill { Skill = csharp },
                    new ProjectSkill { Skill = aspNet },
                    new ProjectSkill { Skill = rest }
                ]
            };

            await context.Projects.AddRangeAsync(
                project1,
                project2);

            await context.SaveChangesAsync();
        }

        const string adminRole = "Admin";

        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(
                new IdentityRole(adminRole));
        }

        //Admin user seeding
        var adminEmail = configuration["AdminUser:Email"];

        var adminPassword = configuration["AdminUser:Password"];

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            return;
        }

        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(
                adminUser,
                adminPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(error => error.Description));

                throw new InvalidOperationException(
                    $"No se pudo crear el usuario administrador: {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(adminUser, adminRole))
        {
            await userManager.AddToRoleAsync(adminUser, adminRole);
        }


    }
}