using App.Domain;

namespace WebApp.ViewModels;

public class IndexCompanyViewModel
{
    public IEnumerable<Company>? Companies { get; set; }
}