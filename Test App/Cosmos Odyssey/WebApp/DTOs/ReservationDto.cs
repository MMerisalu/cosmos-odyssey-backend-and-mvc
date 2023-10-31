using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace WebApp.DTOs;

public class ReservationDto
{
    public Guid Id { get; set; }
    [Required]
    [MaxLength(64)]
    [StringLength(64, MinimumLength = 1)]
    public string FirstName { get; set; } = default!;
    
    [Required]
    [MaxLength(64)]
    [StringLength(64, MinimumLength = 1)]
    public string LastName { get; set; } = default!;

    
    
    [Required] 

    public string From { get; set; } = default!;
    
    [Required] 
    public string? To { get; set; }

    public ICollection<FlightRouteDto> Routes { get; set; } = new HashSet<FlightRouteDto>();

    public Guid PriceListId { get; set; }
    public decimal? TotalQuotedPrice { get; set; }
    public long? TotalDistance { get; set; }
    public TimeSpan? TotalTravelTime { get; set; }
    public string? CompanyNames { get; set; } 
    public string LayOvers { get; set; } = default!;
}