using Portfolio.Domain.Entities;

namespace Portfolio.Application.Services;

public interface IProjectService
{
    Task<IEnumerable<Project>> GetAllAsync();

    Task<Project?> GetByIdAsync(int id);

    Task<Project> CreateAsync(Project project);

    Task<bool> UpdateAsync(Project project);

    Task<bool> DeleteAsync(int id);
}