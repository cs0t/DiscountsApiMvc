namespace Discounts.Application.Models;

public class SellerDashboardStats
{
    public int TotalOffers { get; set; }
    public int ApprovedOffers { get; set; }
    public int PendingOffers { get; set; }
    public int ExpiredOffers { get; set; }
    public int TotalCouponsSold { get; set; }
    public decimal TotalIncome { get; set; }
}