using Portfolio.Domain.Entities;

namespace Portfolio.Application.Services;

public interface ISkillService
{
    Task<IEnumerable<Skill>> GetAllAsync();

    Task<Skill?> GetByIdAsync(int id);

    Task<Skill> CreateAsync(Skill skill);

    Task<bool> UpdateAsync(Skill skill);

    Task<bool> DeleteAsync(int id);
}