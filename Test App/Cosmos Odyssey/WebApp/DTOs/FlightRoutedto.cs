using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace WebApp.DTOs;

public class FlightRouteDto
{
    public Guid FlightRouteId { get; set; }
    
    public string From { get; set; } = default!;


    public string To { get; set; } = default!;

    public long Distance { get; set; }
    
    public Guid CompanyId { get; set; }
    public string? CompanyName { get; set; }
    
    
    
    public decimal Price { get; set; }
    public DateTimeOffset FlightStart { get; set; }
    public DateTimeOffset FlightEnd { get; set; }

    
    public TimeSpan TravelTime { get; set; }
    
    public Guid PriceListId { get; set; }
    public Guid ProviderId { get; set; }
}