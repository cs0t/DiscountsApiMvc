using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Discounts.MVC.ViewModels.Seller;

public class CreateOfferViewModel
{
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = null!;

    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Original price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Original price must be positive")]
    [Display(Name = "Original Price")]
    public decimal OriginalPrice { get; set; }

    [Required(ErrorMessage = "Discounted price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Discounted price must be positive")]
    [Display(Name = "Discounted Price")]
    public decimal DiscountedPrice { get; set; }

    [Required(ErrorMessage = "Max quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Max quantity must be at least 1")]
    [Display(Name = "Max Quantity")]
    public int MaxQuantity { get; set; }

    [Required(ErrorMessage = "Remaining quantity is required")]
    [Range(0, int.MaxValue)]
    [Display(Name = "Remaining Quantity")]
    public int RemainingQuantity { get; set; }

    [Required(ErrorMessage = "Expiration date is required")]
    [DataType(DataType.Date)]
    [Display(Name = "Expiration Date")]
    public DateTime ExpirationDate { get; set; }

    [Required(ErrorMessage = "Select at least one category")]
    [Display(Name = "Categories")]
    public List<int> CategoryIds { get; set; } = new();

    public List<SelectListItem>? AvailableCategories { get; set; }
}

