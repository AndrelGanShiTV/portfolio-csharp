using System.ComponentModel.DataAnnotations;

namespace Portfolio.Web.ViewModels.Experiences;

public class ExperienceViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    [Display(Name = "Empresa")]
    public string Company { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    [Display(Name = "Cargo")]
    public string Position { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    [Display(Name = "Descripción")]
    public string Description { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de inicio")]
    public DateOnly StartDate { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Fecha de finalización")]
    public DateOnly? EndDate { get; set; }

    [Display(Name = "Trabajo actual")]
    public bool IsCurrent { get; set; }

    [Display(Name = "Orden")]
    public int DisplayOrder { get; set; }
}