using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App.Domain;

namespace WebApp.ViewModels;

public class IndexReservationViewModel
{
    [DisplayName("Reservation ID")]
    public Guid Id { get; set; }
    
    [DisplayName("First Name")]
    public string FirstName { get; set; } = default!;
    
    
    [DisplayName("Last Name")]
    public string LastName { get; set; } = default!;
    
    public string From { get; set; } = default!;
    
    public string? To { get; set; }
    
    [DisplayName("Total Price")]
    public decimal TotalPrice { get; set; }

    [DisplayName("Total Flight Time")]
    
    public TimeSpan TotalFlightTime { get; set; }
    public string TotalFlightTimeFormatted => $"{TotalFlightTime.Days} days, {TotalFlightTime.Hours} hours, {TotalFlightTime.Minutes} minutes";
    
    [DisplayName("Company Names")]
    public string? CompanyNames { get; set; }

    public List<Reservation>? Reservations { get; set; }

}