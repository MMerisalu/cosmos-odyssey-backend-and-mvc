
namespace WebApp.DTOs;

public class PriceListDto
{
    public Guid Id { get; set; }
    public int NumberOfReservation { get; set; }
    public string ValidUntil { get; set; } = default!;
    public int NumberOfLegs { get; set; }
}