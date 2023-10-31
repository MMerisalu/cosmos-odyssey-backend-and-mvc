using System.ComponentModel;
using Base.Domain;

namespace App.Domain;

public class PriceList: DomainEntityId
{
   
    public List<Reservation>? Reservations { get; set; }
    [DisplayName("Valid Until")]
    public DateTimeOffset ValidUntil { get; set; }
    public ICollection<RouteInfo>? Legs { get; set; }
}