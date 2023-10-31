using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels;

public class CreateProviderViewModel
{
    public Guid Id { get; set; }

    [DisplayFormat(DataFormatString = "{0:C2}")]
    [DataType(DataType.Currency)]
    public string Price { get; set; } = default!;

    [DataType(DataType.DateTime)] 
    public string FlightStart { get; set; } = default!;

    [DataType(DataType.DateTime)] 
    public string FlightEnd { get; set; } = default!;

    
    public int TravelTime { get; set; }
}