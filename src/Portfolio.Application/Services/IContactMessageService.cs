using Portfolio.Domain.Entities;
using Portfolio.Application.Common;

namespace Portfolio.Application.Services;

public interface IContactMessageService
{
    Task<IEnumerable<ContactMessage>> GetAllAsync();

    Task<PagedResult<ContactMessage>> GetPagedAsync(int page, int pageSize);

    Task<ContactMessage?> GetByIdAsync(int id);

    Task<ContactMessage> CreateAsync(ContactMessage message);

    Task<bool> MarkAsReadAsync(int id);

    Task<bool> DeleteAsync(int id);
}