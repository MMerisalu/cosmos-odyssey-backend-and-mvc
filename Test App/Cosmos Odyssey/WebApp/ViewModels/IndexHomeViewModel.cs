using System.ComponentModel;

namespace WebApp.ViewModels;

public class IndexHomeViewModel
{
    [DisplayName("Reservation ID")]
    public Guid Id { get; set; }
    
    [DisplayName("Last Name")]
    public string? LookupLastName { get; set; }  
}