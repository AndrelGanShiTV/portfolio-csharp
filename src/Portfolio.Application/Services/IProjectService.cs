using Portfolio.Domain.Entities;

namespace Portfolio.Application.Services;

public interface IProjectService
{
    Task<IEnumerable<Project>> GetAllForAdminAsync();

    Task<IEnumerable<Project>> GetAllAsync();

    Task<Project?> GetByIdForAdminAsync(int id);

    Task<Project?> GetByIdAsync(int id);

    Task<Project> CreateAsync(Project project);

    Task<bool> UpdateAsync(Project project, IEnumerable<int> selectedSkillIds);

    Task<bool> DeleteAsync(int id);
}