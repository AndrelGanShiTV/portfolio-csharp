using System.ComponentModel.DataAnnotations;

namespace Portfolio.Web.ViewModels.Skills;

public class SkillViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Tecnología")]
    public string Name { get; set; } = string.Empty;
}