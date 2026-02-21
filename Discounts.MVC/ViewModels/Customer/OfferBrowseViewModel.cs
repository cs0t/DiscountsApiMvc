using Microsoft.AspNetCore.Mvc.Rendering;

namespace Discounts.MVC.ViewModels.Customer;

public class OfferBrowseViewModel
{
    public decimal? PriceStart { get; set; }
    public decimal? PriceEnd { get; set; }
    public List<int>? CategoryIds { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 8;

    public List<SelectListItem>? AvailableCategories { get; set; }
}

