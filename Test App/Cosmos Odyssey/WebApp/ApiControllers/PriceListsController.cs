using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using WebApp.DTOs;
using WebApp.Helpers;

namespace WebApp.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriceListsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _client;

        public PriceListsController(AppDbContext context)
        {
            _context = context;
            _client = new HttpClient();
        }

        // GET: api/PriceLists
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PriceListDto>>> GetPriceLists()
        {
            return await GetQuery().ToListAsync();
        }

        private IQueryable<PriceListDto> GetQuery()
        {
            return _context.PriceLists.Select(p => new PriceListDto
            {
                Id = p.Id,
                NumberOfReservation = p.Reservations!.Count,
                ValidUntil = p.ValidUntil.ToString("g"),
                NumberOfLegs = p.Legs!.Count
            });
        }

        // GET: api/PriceLists/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PriceListDto>> GetPriceList(Guid id)
        {
            var priceList = await GetQuery().FirstOrDefaultAsync(p => p.Id == id);

            if (priceList == null)
            {
                return NotFound();
            }

            return priceList;
        }


        // DELETE: api/PriceLists/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePriceList(Guid id)
        {
            var priceList = await _context.PriceLists.FindAsync(id);
            if (priceList == null)
            {
                return NotFound();
            }

            _context.PriceLists.Remove(priceList);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /*
        private bool PriceListExists(Guid id)
        {
            return (_context.PriceLists?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        */

        [HttpGet("refresh")]
        public async Task<PriceListDto> Refresh()
        {
            var priceList = await PriceListHelper.RefreshPriceList(_context, _client);
            return (await GetQuery().FirstOrDefaultAsync(p => p.Id == priceList.Id))!;
        }

        [HttpGet("{priceListId}/GetAllOrigins")]
        public async Task<List<string>?> GetAllOrigins(Guid priceListId)
        {
            return await _context.PriceLists.Where(x => x.Id == priceListId)
                .SelectMany(p => p.Legs!.Select(l => l.From))
                .Distinct()
                .ToListAsync();
        }

        [HttpGet("{priceListId}/GetLegs/{from}")]
        public async Task<List<FlightRouteDto>?> GetLegsFromOrigins(Guid priceListId, string from, DateTimeOffset? minDepartureTime)
        {
            var query = _context.Providers
                .Include(p => p.RouteInfo)
                .Include(p => p.Company)
                .Where(p => p.RouteInfo!.PriceListId.Equals(priceListId))
                .Where(p => p.RouteInfo!.From.Equals(from));

            if(minDepartureTime.HasValue)
                query = query.Where(x => x.FlightStart >= minDepartureTime);

            return await query.Select(p => new FlightRouteDto
                {
                    PriceListId = p.RouteInfo!.PriceListId,
                    ProviderId = p.Id,
                    FlightRouteId = p.RouteInfoId,
                    CompanyId = p.CompanyId,
                    CompanyName = p.Company!.Name,
                    FlightStart = p.FlightStart,
                    FlightEnd = p.FlightEnd,
                    From = p.RouteInfo.From,
                    To = p.RouteInfo.To!,
                    Distance =Math.Abs(p.RouteInfo.Distance),
                    Price = p.Price,
                    TravelTime = p.TravelTime
                })
            .ToListAsync();
        }
    }


}
