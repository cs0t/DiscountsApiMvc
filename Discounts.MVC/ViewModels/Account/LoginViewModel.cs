using System.ComponentModel.DataAnnotations;

namespace Discounts.MVC.ViewModels.Account;

public class LoginViewModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email must be a valid email address")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    public string? ReturnUrl { get; set; }
}

