namespace Portfolio.Web.ViewModels.Contact;

public class ContactMessageAdminViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }

    public bool IsRead { get; set; }
}