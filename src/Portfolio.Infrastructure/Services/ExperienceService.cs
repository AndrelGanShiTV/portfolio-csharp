using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Services;
using Portfolio.Domain.Entities;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Infrastructure.Services;

public class ExperienceService : IExperienceService
{
    private readonly AppDbContext _context;

    public ExperienceService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Experience>> GetAllAsync()
    {
        return await _context.Experiences
            .OrderBy(experience => experience.DisplayOrder)
            .ThenByDescending(experience => experience.StartDate)
            .ToListAsync();
    }

    public async Task<Experience?> GetByIdAsync(int id)
    {
        return await _context.Experiences
            .FirstOrDefaultAsync(experience => experience.Id == id);
    }

    public async Task<Experience> CreateAsync(
        Experience experience)
    {
        _context.Experiences.Add(experience);

        await _context.SaveChangesAsync();

        return experience;
    }

    public async Task<bool> UpdateAsync(
        Experience experience)
    {
        var existingExperience =
            await _context.Experiences
                .FirstOrDefaultAsync(existing =>
                    existing.Id == experience.Id);

        if (existingExperience is null)
        {
            return false;
        }

        existingExperience.Company = experience.Company;
        existingExperience.Position = experience.Position;
        existingExperience.Description = experience.Description;
        existingExperience.StartDate = experience.StartDate;
        existingExperience.EndDate = experience.EndDate;
        existingExperience.IsCurrent = experience.IsCurrent;
        existingExperience.DisplayOrder = experience.DisplayOrder;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var experience = await _context.Experiences
            .FirstOrDefaultAsync(experience =>
                experience.Id == id);

        if (experience is null)
        {
            return false;
        }

        _context.Experiences.Remove(experience);

        await _context.SaveChangesAsync();

        return true;
    }
}