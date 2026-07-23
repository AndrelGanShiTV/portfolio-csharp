using Portfolio.Domain.Entities;

namespace Portfolio.Application.Services;

public interface IProjectService
{
    IEnumerable<Project> GetAll();

    Project? GetById(int id);
}