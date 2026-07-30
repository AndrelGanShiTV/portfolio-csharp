using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Services;
using Portfolio.Domain.Entities;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Infrastructure.Services;

public class SkillService : ISkillService
{
    private readonly AppDbContext _context;

    public SkillService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Skill>> GetAllAsync()
    {
        return await _context.Skills
            .OrderBy(skill => skill.Name)
            .ToListAsync();
    }

    public async Task<Skill?> GetByIdAsync(int id)
    {
        return await _context.Skills
            .FirstOrDefaultAsync(skill => skill.Id == id);
    }

    public async Task<Skill> CreateAsync(Skill skill)
    {
        _context.Skills.Add(skill);

        await _context.SaveChangesAsync();

        return skill;
    }

    public async Task<bool> UpdateAsync(Skill skill)
    {
        var existingSkill = await _context.Skills
            .FirstOrDefaultAsync(x => x.Id == skill.Id);

        if (existingSkill is null)
        {
            return false;
        }

        existingSkill.Name = skill.Name;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var skill = await _context.Skills
            .FirstOrDefaultAsync(x => x.Id == id);

        if (skill is null)
        {
            return false;
        }

        _context.Skills.Remove(skill);

        await _context.SaveChangesAsync();

        return true;
    }
}