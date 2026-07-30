using Portfolio.Domain.Entities;

namespace Portfolio.Application.Services;

public interface IContactMessageService
{
    Task<IEnumerable<ContactMessage>> GetAllAsync();

    Task<ContactMessage?> GetByIdAsync(int id);

    Task<ContactMessage> CreateAsync(ContactMessage message);

    Task<bool> MarkAsReadAsync(int id);

    Task<bool> DeleteAsync(int id);
}