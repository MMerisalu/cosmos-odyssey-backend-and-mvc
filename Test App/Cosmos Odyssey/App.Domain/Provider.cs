using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Base.Domain;

namespace App.Domain;

public class Provider : DomainEntityId
{
    public Guid CompanyId { get; set; }
    public Company? Company { get; set; }
    public Guid RouteInfoId { get; set; }
    public RouteInfo? RouteInfo { get; set; }

    public ICollection<FlightRoute>? Flights { get; set; } = new HashSet<FlightRoute>();
    
    [DisplayFormat(DataFormatString = "{0:N2}")]
    
    public decimal Price { get; set; }
    
    [DisplayName("Flight Start")]
    public DateTimeOffset FlightStart { get; set; }
    
    [DisplayName("Flight End")]
    public DateTimeOffset FlightEnd { get; set; }

    [DataType(DataType.Duration)]
    
    [DisplayName("Travel Time")]
    public TimeSpan TravelTime { get; set; }
}