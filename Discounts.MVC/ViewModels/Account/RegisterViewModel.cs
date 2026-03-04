using System.ComponentModel.DataAnnotations;

namespace Discounts.MVC.ViewModels.Account;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 20 characters")]
    public string UserName { get; set; } = null!;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email must be a valid email address")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Confirm Password is required")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Confirm Password must be at least 6 characters")]
    [Compare("Password", ErrorMessage = "Confirm Password must match Password")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = null!;
}

