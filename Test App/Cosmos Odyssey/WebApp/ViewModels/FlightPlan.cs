using App.Domain;

namespace WebApp.ViewModels;

public class FlightPlan
{
    public RouteInfo[]? Legs { get; set; }
    
    public decimal TotalPrice { get; set; }
    public TimeSpan TotalTravelTime { get; set; }
    public string? CompanyNames { get; set; }
}