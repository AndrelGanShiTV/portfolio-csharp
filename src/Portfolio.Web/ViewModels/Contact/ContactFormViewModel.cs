using System.ComponentModel.DataAnnotations;

namespace Portfolio.Web.ViewModels.Contact;

public class ContactFormViewModel
{
    [Required]
    [StringLength(120)]
    [Display(Name = "Nombre")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(200)]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    [Display(Name = "Asunto")]
    public string Subject { get; set; } = string.Empty;

    [Required]
    [StringLength(3000)]
    [Display(Name = "Mensaje")]
    public string Message { get; set; } = string.Empty;
}