using System.ComponentModel.DataAnnotations;

namespace Discounts.MVC.ViewModels.Admin;

public class CreateSystemSettingViewModel
{
    [Required(ErrorMessage = "Key is required")]
    [StringLength(100)]
    public string Key { get; set; } = null!;

    [Required(ErrorMessage = "Value is required")]
    [StringLength(500)]
    [Display(Name = "Setting Value")]
    public string SettingValue { get; set; } = null!;
}

