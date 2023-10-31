using App.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels;

public class IndexProviderViewModel
{
    public IEnumerable<Provider>? Providers { get; set; }
    public SelectList? PriceLists { get; set; }
    public SelectListItem? SelectedItem { get; set; }
    public string? CompanySearch { get; set; }
    public SelectList? SortOptions { get; set; }
    public SelectListItem? SelectedSortOption { get; set; }
    public Guid? SelectedPriceListId { get; set; }
}