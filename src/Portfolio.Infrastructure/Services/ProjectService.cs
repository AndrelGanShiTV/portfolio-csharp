using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Services;
using Portfolio.Domain.Entities;
using Portfolio.Infrastructure.Persistence;
using Portfolio.Application.Common;

namespace Portfolio.Infrastructure.Services;

public class ProjectService : IProjectService
{
    private readonly AppDbContext _context;

    public ProjectService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Project>> GetAllForAdminAsync()
    {
        return await _context.Projects
            .Include(project => project.ProjectSkills)
            .ThenInclude(projectSkill => projectSkill.Skill)
            .OrderBy(project => project.Name)
            .ToListAsync();
    }

    public async Task<PagedResult<Project>> GetPagedForAdminAsync(
    int page,
    int pageSize)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _context.Projects
            .Include(project => project.ProjectSkills)
                .ThenInclude(projectSkill => projectSkill.Skill)
            .OrderByDescending(project => project.Id);

        var totalItems = await query.CountAsync();

        var totalPages = (int)Math.Ceiling(
            totalItems / (double)pageSize);

        if (totalPages > 0 && page > totalPages)
        {
            page = totalPages;
        }

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Project>
        {
            Items = items,
            CurrentPage = page,
            TotalPages = totalPages,
            TotalItems = totalItems
        };
    }

    public async Task<IEnumerable<Project>> GetAllAsync()
    {
        return await _context.Projects
            .Include(project => project.ProjectSkills)
            .ThenInclude(projectSkill => projectSkill.Skill)
            .Where(project => project.IsPublished)
            .ToListAsync();
    }

    public async Task<Project?> GetByIdForAdminAsync(int id)
    {
        return await _context.Projects
            .Include(project => project.ProjectSkills)
                .ThenInclude(projectSkill => projectSkill.Skill)
            .FirstOrDefaultAsync(project => project.Id == id);
    }

    public async Task<Project?> GetByIdAsync(int id)
    {
        return await _context.Projects
            .Include(project => project.ProjectSkills)
            .ThenInclude(projectSkill => projectSkill.Skill)
            .FirstOrDefaultAsync(project =>
                project.Id == id &&
                project.IsPublished);
    }

    public async Task<Project> CreateAsync(Project project)
    {
        _context.Projects.Add(project);

        await _context.SaveChangesAsync();

        return project;
    }

    public async Task<bool> UpdateAsync(Project project, IEnumerable<int> selectedSkillIds)
    {
        var existingProject = await _context.Projects
            .Include(x => x.ProjectSkills)
            .FirstOrDefaultAsync(x => x.Id == project.Id);

        if (existingProject is null)
        {
            return false;
        }

        existingProject.Name = project.Name;
        existingProject.Description = project.Description;
        existingProject.RepositoryUrl = project.RepositoryUrl;
        existingProject.DemoUrl = project.DemoUrl;
        existingProject.IsPublished = project.IsPublished;

        var distinctSkillIds = selectedSkillIds
            .Distinct()
            .ToHashSet();

        var relationshipsToRemove = existingProject.ProjectSkills
            .Where(projectSkill => !distinctSkillIds.Contains(projectSkill.SkillId))
            .ToList();

        _context.ProjectSkills.RemoveRange(relationshipsToRemove);

        var existingSkillIds = existingProject.ProjectSkills
            .Select(projectSkill => projectSkill.SkillId)
            .ToHashSet();

        foreach (var skillId in distinctSkillIds)
        {
            if (!existingSkillIds.Contains(skillId))
            {
                existingProject.ProjectSkills.Add(new ProjectSkill
                {
                    ProjectId = existingProject.Id,
                    SkillId = skillId
                });
            }
        }

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(project => project.Id == id);

        if (project is null)
        {
            return false;
        }

        _context.Projects.Remove(project);

        await _context.SaveChangesAsync();

        return true;
    }
}