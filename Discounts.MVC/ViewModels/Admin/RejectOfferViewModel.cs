using System.ComponentModel.DataAnnotations;

namespace Discounts.MVC.ViewModels.Admin;

public class RejectOfferViewModel
{
    public int OfferId { get; set; }
    
    public string? OfferTitle { get; set; }

    [Required(ErrorMessage = "Reason is required.")]
    [MaxLength(500, ErrorMessage = "Reason must not exceed 500 characters.")]
    public string Reason { get; set; } = null!;
}

