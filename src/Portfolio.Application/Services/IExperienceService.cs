using Portfolio.Domain.Entities;

namespace Portfolio.Application.Services;

public interface IExperienceService
{
    Task<IEnumerable<Experience>> GetAllAsync();

    Task<Experience?> GetByIdAsync(int id);

    Task<Experience> CreateAsync(Experience experience);

    Task<bool> UpdateAsync(Experience experience);

    Task<bool> DeleteAsync(int id);
}