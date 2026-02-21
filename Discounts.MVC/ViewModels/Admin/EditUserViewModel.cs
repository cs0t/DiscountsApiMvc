using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Discounts.MVC.ViewModels.Admin;

public class EditUserViewModel
{
    public int UserId { get; set; }

    [StringLength(50, MinimumLength = 3)]
    public string? UserName { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6)]
    public string? Password { get; set; }

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    [Display(Name = "Confirm Password")]
    public string? ConfirmPassword { get; set; }

    [Display(Name = "Role")]
    public int? RoleId { get; set; }

    public List<SelectListItem>? Roles { get; set; }
}

