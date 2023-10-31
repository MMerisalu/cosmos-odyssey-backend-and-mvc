using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class Company : DomainEntityId
{
    [Required]
    [MaxLength(64)]
    [StringLength(64, MinimumLength = 1)]
    public string Name { get; set; } = default!;

    public ICollection<Provider>? Providers { get; set; }

}