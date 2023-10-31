using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels;

public class CreateFromViewModel
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(64)]
    [StringLength(64, MinimumLength = 1)]
    public string Name { get; set; } = default!;
}