using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Discounts.MVC.ViewModels.Admin;

public class CreateUserViewModel
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 20 characters")]
    public string UserName { get; set; } = null!;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email must be a valid email address")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Confirm Password is required")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Confirm Password must match Password")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = null!;

    [Required(ErrorMessage = "Role is required")]
    [Range(1, int.MaxValue, ErrorMessage = "RoleId must be a positive integer")]
    [Display(Name = "Role")]
    public int RoleId { get; set; }

    public List<SelectListItem>? Roles { get; set; }
}

