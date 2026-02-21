using System.ComponentModel.DataAnnotations;

namespace Discounts.MVC.ViewModels.Admin;

public class EditSystemSettingViewModel
{
    public int Id { get; set; }

    public string Key { get; set; } = null!;

    [Required(ErrorMessage = "Value is required")]
    [StringLength(500)]
    [Display(Name = "Setting Value")]
    public string NewSettingValue { get; set; } = null!;
}

