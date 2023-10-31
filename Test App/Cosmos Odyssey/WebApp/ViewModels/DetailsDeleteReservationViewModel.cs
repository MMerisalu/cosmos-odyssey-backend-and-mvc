using System.ComponentModel;

namespace WebApp.ViewModels;

public class DetailsDeleteReservationViewModel
{
    [DisplayName("Reservation ID")]
    public Guid Id { get; set; }
    
    [DisplayName("First Name")]
    public string FirstName { get; set; } = default!;
    
    [DisplayName("Last Name")]
    public string LastName { get; set; } = default!;
    public string From { get; set; } = default!;
    public string To { get; set; } = default!;
    
    [DisplayName("Total Price")]
    public string TotalPrice { get; set; } = default!;
    
    [DisplayName("Total Flight Time")]
    public string TotalFlightTime { get; set; } = default!;
    public string Routes { get; set; } = default!;
    public string LayOvers { get; set; } = default!;
    
    [DisplayName("Company Names")]
    public string? CompanyNames { get; set; } = default!;
}