using System.ComponentModel.DataAnnotations;

namespace Discounts.MVC.ViewModels.Admin;

public class CreateCategoryViewModel
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = null!;

    [MinLength(5, ErrorMessage = "Description must be at least 5 characters long!")]
    public string? Description { get; set; }
}

