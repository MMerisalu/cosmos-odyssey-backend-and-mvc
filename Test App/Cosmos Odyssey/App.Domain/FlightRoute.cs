using System.ComponentModel.DataAnnotations.Schema;
using Base.Domain;

namespace App.Domain;

public class FlightRoute : DomainEntityId
{
    [ForeignKey(nameof(Reservation))]
    public Guid ReservationId { get; set; }
    public Reservation? Reservation { get; set; }

    [ForeignKey(nameof(Provider))]
    public Guid ProviderId { get; set; }
    public Provider? Provider { get; set; }
}