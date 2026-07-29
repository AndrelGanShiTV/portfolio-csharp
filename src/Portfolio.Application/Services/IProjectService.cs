using Portfolio.Domain.Entities;

namespace Portfolio.Application.Services;

public interface IProjectService
{
    Task<IEnumerable<Project>> GetAllAsync();

    Task<Project?> GetByIdAsync(int id);
}