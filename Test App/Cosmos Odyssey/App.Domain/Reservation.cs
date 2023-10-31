using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class Reservation : DomainEntityId
{
    [Required]
    [MaxLength(64)]
    [StringLength(64, MinimumLength = 1)]
    [DisplayName("First Name")]
    public string FirstName { get; set; } = default!;
    
    [Required]
    [MaxLength(64)]
    [StringLength(64, MinimumLength = 1)]
    [DisplayName("Last Name")]
    public string LastName { get; set; } = default!;

    public string FirstAndLastName => $"{FirstName} {LastName}";
    public string LastAndFirstName => $"{LastName} {FirstName}";
    
    [Required] 

    public string From { get; set; } = default!;
    
    [Required] 
    public string? To { get; set; }
    
    public ICollection<FlightRoute> Routes { get; set; } = new HashSet<FlightRoute>();

    public Guid PriceListId { get; set; }
    public PriceList? PriceList { get; set; }
    
    [DisplayName("Total Price")]
    public decimal TotalPrice { get; set; }

    [DisplayName("Total Flight Time")]
    public TimeSpan TotalFlightTime { get; set; }

    
    public string TotalFlightTimeFormatted =>
        $"{TotalFlightTime.Days} days, {TotalFlightTime.Hours} hours, {TotalFlightTime.Minutes} minutes";

    [DisplayName("Company Names")] 
    
    public string? Companies { get; set; }
    public string? LayOvers { get;set; } 











}