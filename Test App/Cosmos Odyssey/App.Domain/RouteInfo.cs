using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Base.Domain;

namespace App.Domain;

public class RouteInfo : DomainEntityId
{
#warning  According to the api scheme to and from schould be classes but 
#warning I feel that its more correct to have them as props
    
    public Guid FromId { get; set; }
    public string From { get; set; } = default!;

    public Guid ToId { get; set; }
    public string? To { get; set; }

    public Int64 Distance { get; set; }
    
    public ICollection<Provider>? Providers { get; set; }
    
    public Guid PriceListId { get; set; }
    public PriceList? PriceList { get; set; }
}