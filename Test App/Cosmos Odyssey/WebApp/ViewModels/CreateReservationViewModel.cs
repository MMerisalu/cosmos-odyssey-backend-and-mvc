using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels;

public class CreateReservationViewModel
{
    public Guid SelectedPriceListId { get; set; }
    
    public Guid Id { get; set; }
    
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

    [Required]
    
    public string?  From { get; set; }
    public SelectList? FromOptions { get; set; }
    
    #region Selected Provider Legs

    public string? SelectedLegIds { get; set; }
    public ICollection<Provider> SelectedLegs { get; set; } = new HashSet<Provider>();
    [DisplayName("Total Quoted Price")]
    public decimal? TotalQuotedPrice { get; set; }
    [DisplayName("Total Distance")]
    public Int128? TotalDistance { get; set; }
    [DisplayName("Total Travel Time")]
    public TimeSpan? TotalTravelTime { get; set; }
    public string? CompanyNames { get; set; }
    
    #endregion Selected Provider Legs
    
    #region Provider Leg Selection 
    public SelectList? SortOptions { get; set; }
    public string? SelectedSortOption { get; set; }
    public string? CompanySearch { get; set; }
    
    public SelectList? LegToOptions { get; set; }
    public string? SelectedLegToOption { get; set; }
    
    public IEnumerable<Provider>? Providers { get; set; }
    #endregion Provider Leg Selection

    public bool IsGetLegsVisible { get; set; }
    
    public bool IsSubmitVisible { get; set; }
    
    
    // This is set through CODE, not the user!
    public string? To { get; set; }
    
}