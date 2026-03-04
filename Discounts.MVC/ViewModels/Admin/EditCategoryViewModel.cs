using System.ComponentModel.DataAnnotations;

namespace Discounts.MVC.ViewModels.Admin;

public class EditCategoryViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [Display(Name = "Name")]
    public string NewName { get; set; } = null!;

    [MinLength(5, ErrorMessage = "Description must be at least 5 characters long!")]
    [Display(Name = "Description")]
    public string? NewDescription { get; set; }
}

