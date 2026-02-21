using System.ComponentModel.DataAnnotations;

namespace Discounts.MVC.ViewModels.Admin;

public class RejectOfferViewModel
{
    public int OfferId { get; set; }
    
    public string? OfferTitle { get; set; }

    [Required(ErrorMessage = "Rejection reason is required")]
    [StringLength(500, MinimumLength = 5, ErrorMessage = "Reason must be between 5 and 500 characters")]
    public string Reason { get; set; } = null!;
}

