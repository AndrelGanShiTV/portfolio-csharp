using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Services;
using Portfolio.Domain.Entities;
using Portfolio.Infrastructure.Persistence;
using Portfolio.Application.Common;

namespace Portfolio.Infrastructure.Services;

public class ContactMessageService : IContactMessageService
{
    private readonly AppDbContext _context;

    public ContactMessageService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ContactMessage>> GetAllAsync()
    {
        return await _context.ContactMessages
            .OrderBy(message => message.IsRead)
            .ThenByDescending(message => message.CreatedAtUtc)
            .ToListAsync();
    }

    public async Task<PagedResult<ContactMessage>> GetPagedAsync(int page, int pageSize)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _context.ContactMessages
            .OrderBy(message => message.IsRead)
            .ThenByDescending(message => message.CreatedAtUtc);

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

        return new PagedResult<ContactMessage>
        {
            Items = items,
            CurrentPage = page,
            TotalPages = totalPages,
            TotalItems = totalItems
        };
    }

    public async Task<ContactMessage?> GetByIdAsync(int id)
    {
        return await _context.ContactMessages
            .FirstOrDefaultAsync(message => message.Id == id);
    }

    public async Task<ContactMessage> CreateAsync(
        ContactMessage message)
    {
        _context.ContactMessages.Add(message);

        await _context.SaveChangesAsync();

        return message;
    }

    public async Task<bool> MarkAsReadAsync(int id)
    {
        var message = await _context.ContactMessages
            .FirstOrDefaultAsync(message => message.Id == id);

        if (message is null)
        {
            return false;
        }

        message.IsRead = true;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var message = await _context.ContactMessages
            .FirstOrDefaultAsync(message => message.Id == id);

        if (message is null)
        {
            return false;
        }

        _context.ContactMessages.Remove(message);

        await _context.SaveChangesAsync();

        return true;
    }
}