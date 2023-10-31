using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain;
using WebApp.DTOs;

namespace WebApp.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReservationsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Reservations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationDto>>> GetReservations()
        {
            return await GetQuery().ToListAsync();
        }

        internal IQueryable<ReservationDto> GetQuery()
        {
            var reservations = _context.Reservations
                .Select(r => new ReservationDto
                {
                    Id = r.Id,
                    FirstName = r.FirstName,
                    LastName = r.LastName,
                    From = r.From,
                    To = r.To,
                    CompanyNames = r.Companies!,
                    LayOvers = r.LayOvers!,
                    PriceListId = r.PriceListId,
                    TotalQuotedPrice = r.TotalPrice,
                    TotalDistance = r.Routes.Sum(l => Math.Abs(l.Provider!.RouteInfo!.Distance)),
                    TotalTravelTime = r.TotalFlightTime,
                    Routes = r.Routes.Select(rt => new FlightRouteDto
                    {
                        PriceListId = rt.Provider!.RouteInfo!.PriceListId,
                        ProviderId = rt.ProviderId,
                        FlightRouteId = rt.Id,
                        CompanyId = rt.Provider.CompanyId,
                        CompanyName = rt.Provider!.Company!.Name,
                        FlightStart = rt.Provider.FlightStart,
                        FlightEnd = rt.Provider.FlightEnd,
                        From = rt.Provider.RouteInfo.From,
                        To = rt.Provider!.RouteInfo!.To!,
                        Distance = Math.Abs(rt.Provider.RouteInfo.Distance),
                        Price = rt.Provider.Price,
                        TravelTime = rt.Provider.TravelTime
                    }).ToList()
                });
            return reservations;
        }
        
        
        // GET: api/Reservations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationDto>> GetReservation(Guid id)
        {
            var reservation = await GetQuery().FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            return reservation;
        }


        // POST: api/Reservations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ReservationDto>> PostReservation(ReservationDto dto)
        {
            // Now create the booking
          var reservation = new Reservation()
          {
              Id = dto.Id,
              PriceListId = dto.PriceListId,

              From = dto.From,
              FirstName = dto.FirstName,
              LastName = dto.LastName, 
          };
          
          // Check that the client actually provides an Id
          if (reservation.Id == Guid.Empty)
              reservation.Id = Guid.NewGuid();

          foreach (var leg in dto.Routes)
          {
              // I will re-load the object from the database, I don't trust the client ;)
              var dbProvider = await _context.Providers
                  .Include(p => p.RouteInfo)
                  .Include(p => p.Company)
                  .FirstAsync(p => p.Id == leg.ProviderId);
              
              reservation.Routes.Add(new FlightRoute
              {
                  Id = Guid.NewGuid(),
                  Provider = dbProvider
              });
          }

          reservation.To = reservation.Routes.Last().Provider!.RouteInfo!.To;
          reservation.TotalPrice = reservation.Routes.Sum(r => r.Provider!.Price);
          reservation.Companies = String.Join(", ", reservation.Routes.Select(r => r.Provider!.Company!.Name));
          
          reservation.TotalFlightTime = TimeSpan.Zero;
          foreach (var leg in reservation.Routes)
              reservation.TotalFlightTime = reservation.TotalFlightTime.Add(leg.Provider!.TravelTime);
          reservation.LayOvers = String.Join(" - ",
              reservation.Routes.Select(x => x.Provider!.RouteInfo!.From)
                  .Concat(new[] { reservation.Routes.Last().Provider!.RouteInfo!.To }));
          _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            var createdReservationDto = await GetQuery().FirstOrDefaultAsync(r => r.Id == reservation.Id);
            return CreatedAtAction("GetReservation", new { id = reservation.Id }, createdReservationDto);
        }

        // DELETE: api/Reservations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(Guid id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        ///  Reservation search
        /// </summary>
        /// <param name="id">Reservation id</param>
        /// <param name="lastName">Reservation's owners last name</param>
        /// <returns>A reservation or 404 exception</returns>
        [HttpGet("LookUp/")]
        public async Task<ActionResult<ReservationDto?>> LookUp(Guid id, string lastName)
        {
            var record =
                await _context.Reservations
                    .Include(r => r.Routes)
                    .ThenInclude(p => p.Provider)
                    .ThenInclude(r => r!.RouteInfo)
                    .FirstOrDefaultAsync(x => x.Id.Equals(id) &&
                                              x.LastName.Equals(lastName));
            if (record == null)
            {
                return NotFound();
            }


            var reservationDto = new ReservationDto()
            {
                Id = record.Id,
                FirstName = record.FirstName,
                LastName = record.LastName,
                From = record.From,
                To = record.To,
                PriceListId = record.PriceListId,
                LayOvers = record.LayOvers!,
                TotalTravelTime = record.TotalFlightTime,
                TotalDistance = record.Routes.Sum(l => Math.Abs(l.Provider!.RouteInfo!.Distance)),
                TotalQuotedPrice = record.TotalPrice,
                CompanyNames = record.Companies
            };
            return reservationDto;

        }
    }
}
