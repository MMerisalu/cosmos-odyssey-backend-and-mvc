using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels;

public class CreateRouteInfoViewModel
{
    public Guid Id { get; set; }
    public Guid FromId { get; set; }
    public List<SelectListItem>? Froms { get; set; }
    public Guid ToId { get; set; }
    public List<SelectListItem>? Tos { get; set; }

    public int Distance { get; set; }
}