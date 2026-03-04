using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Discounts.MVC.ViewModels.Admin;

public class EditUserViewModel
{
    public int UserId { get; set; }

    [StringLength(20, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 20 characters")]
    public string? UserName { get; set; }

    [EmailAddress(ErrorMessage = "Email must be valid")]
    public string? Email { get; set; }

    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    public string? Password { get; set; }

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Confirm Password must match Password")]
    [Display(Name = "Confirm Password")]
    public string? ConfirmPassword { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "RoleId must be a positive integer")]
    [Display(Name = "Role")]
    public int? RoleId { get; set; }

    public List<SelectListItem>? Roles { get; set; }
}

