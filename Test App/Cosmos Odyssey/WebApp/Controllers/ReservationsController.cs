using System.Globalization;
using App.DAL.EF;
using App.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Helpers;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _client;

        public ReservationsController(AppDbContext context)
        {
            _context = context;
            _client = new HttpClient();
        }

        // GET: Reservations
        public async Task<IActionResult> Index()
        {
            var vm = new IndexReservationViewModel();

            var reservations = await _context.Reservations
                .ToListAsync(); 
            vm.Reservations = reservations;
            
           return View(vm);
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            var vm = new DetailsDeleteReservationViewModel();
            
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            vm.Id = reservation.Id;
            vm.FirstName = reservation.FirstName;
            vm.LastName = reservation.LastName;
            vm.From = reservation.From;
            if (reservation.To != null) vm.To = reservation.To;
            vm.TotalPrice = reservation.TotalPrice.ToString(CultureInfo.CurrentCulture);
            vm.TotalFlightTime = reservation.TotalFlightTimeFormatted;
            if (reservation.LayOvers != null) vm.LayOvers = reservation.LayOvers;
            vm.CompanyNames = reservation.Companies;

            return View(vm);
        }

        // GET: Reservations/Create
        public async Task<IActionResult> Create()
        {
            // Make sure we have the latest pricelist first
            var priceList = await PriceListHelper.RefreshPriceList(_context, _client);

            var currentPriceListId = priceList.Id;
            var vm = new CreateReservationViewModel()
            {
                SelectedPriceListId = currentPriceListId,
            };

            // By default, sort flights by Price
            vm.SelectedSortOption = nameof(Provider.Price);
            vm.SelectedLegToOption = String.Empty;

            vm = await ApplyFiltering(vm, false);
            return View(vm);
        }

        private async Task<CreateReservationViewModel> ApplyFiltering(CreateReservationViewModel vm,
            bool showLegs = true)
        {
            ModelState.Clear();
            // prepare the lookup list sources
            var startPoints = _context.RouteInfos.Where(x => x.PriceListId == vm.SelectedPriceListId)
                .Select(x => x.From)
                .Distinct()
                .ToList();
            vm.FromOptions = new SelectList(startPoints);
            var sortOptions = new List<string>
            {
                nameof(Provider.Price), nameof(Provider.Price) + " (reverse)",
                nameof(Provider.RouteInfo.Distance), nameof(Provider.RouteInfo.Distance) + " (reverse)",
                nameof(Provider.TravelTime), nameof(Provider.TravelTime) + " (reverse)"
            };
            vm.SortOptions = new SelectList(sortOptions);
            vm.SelectedLegIds = String.Join(",", vm.SelectedLegs.Select(x => x.Id));

            if (!String.IsNullOrWhiteSpace(vm.From))
            {
                if (vm.SelectedLegs.Any())
                {
                    // Rebuild the readonly metadata fields
                    vm.To = vm.SelectedLegs.Last().RouteInfo!.To;
                    vm.TotalQuotedPrice = vm.SelectedLegs.Sum(p => p.Price);
                    vm.TotalDistance = 0;
                    vm.TotalTravelTime = TimeSpan.Zero;
                    foreach (var leg in vm.SelectedLegs)
                    {
                        vm.TotalTravelTime = vm.TotalTravelTime.Value.Add(leg.TravelTime);
                        vm.TotalDistance =  vm.TotalDistance.Value + Math.Abs(leg.RouteInfo!.Distance);
                    }

                    vm.CompanyNames = String.Join(", ", vm.SelectedLegs.Select(l => l.Company!.Name).Distinct());
                }

                if (showLegs)
                {

                    // build list of providers for the next leg
                    var query = _context.Providers
                        .Include(p => p.RouteInfo)
                        .Include(p => p.Company)
                        .Where(p => p.RouteInfo!.PriceListId.Equals(vm.SelectedPriceListId));

                    if (String.IsNullOrWhiteSpace(vm.To))
                        query = query.Where(p => p.RouteInfo!.From.Equals(vm.From));
                    else
                        query = query.Where(p => p.RouteInfo!.From.Equals(vm.To)); // start from the previous leg end ;)

                    // Filter by the arrival time of the last leg
                    var lastFlight = vm.SelectedLegs.LastOrDefault();
                    if (lastFlight != null)
                    {
                        var transitTime = TimeSpan.FromMinutes(15);
                        var minDepartureTime = lastFlight.FlightEnd.Add(transitTime);
                        query = query.Where(x => x.FlightStart >= minDepartureTime);
                    }

                    // build the from options, we could do this after company search... but I think we want the full list
                    var legs = await query.Select(p => p.RouteInfo!.To).Distinct().ToListAsync();
                    var legItems = legs.Select(x => new { Value = x, Label = x }).ToList();
                    legItems.Insert(0, new { Value = String.Empty, Label = "All" }!);
                    vm.LegToOptions = new SelectList(legItems, "Value", "Label");

                    if (!String.IsNullOrWhiteSpace(vm.CompanySearch))
                        query = query.Where(p =>
                            p.Company!.Name.Contains(vm.CompanySearch.ToLower()));

                    if (!String.IsNullOrWhiteSpace(vm.SelectedLegToOption))
                        query = query.Where(p => p.RouteInfo!.To!.Equals(vm.SelectedLegToOption));

                    switch (vm.SelectedSortOption ?? String.Empty)
                    {
                        case nameof(Provider.Price):
                            query = query.OrderBy(x => x.Price);
                            break;
                        case nameof(Provider.Price) + " (reverse)":
                            query = query.OrderByDescending(x => x.Price);
                            break;
                        case nameof(Provider.TravelTime):
                            query = query.OrderBy(x => x.TravelTime);
                            break;
                        case nameof(Provider.TravelTime) + " (reverse)":
                            query = query.OrderByDescending(x => x.TravelTime);
                            break;
                        case nameof(Provider.RouteInfo.Distance):
                            query = query.OrderBy(x => x.RouteInfo!.Distance);
                            break;
                        case nameof(Provider.RouteInfo.Distance) + " (reverse)":
                            query = query.OrderByDescending(x => x.RouteInfo!.Distance);
                            break;
                    }

                    vm.Providers = await query.ToListAsync();

                    vm.IsGetLegsVisible = false;
                    vm.IsSubmitVisible = false;
                }
                else
                {
                    vm.Providers = null;
                    vm.IsGetLegsVisible = true;
                    vm.IsSubmitVisible = vm.SelectedLegs.Any();

                }
            }
            else
            {
                vm.IsGetLegsVisible = true;
                vm.IsSubmitVisible = false;
            }

            return vm;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetLegs(CreateReservationViewModel vm)
        {
            // rebuild the previously selected legs from the selected IDs
            await RebuildSelectedLegs(vm);
            // Rebuild the ViewModel, include the provider legs to choose from
            vm = await ApplyFiltering(vm);
            return View(nameof(Create), vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelSelectLeg(CreateReservationViewModel vm)
        {
            // rebuild the previously selected legs from the selected IDs
            await RebuildSelectedLegs(vm);
            // Rebuild the ViewModel, DO NOT show the legs ;)
            vm = await ApplyFiltering(vm, false);
            return View(nameof(Create), vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLeg([FromRoute] Guid id, CreateReservationViewModel vm)
        {
            // rebuild the previously selected legs from the selected IDs
            await RebuildSelectedLegs(vm);

            // Get the reference to the new selected leg
            var selectedLeg = _context.Providers
                .Include(x => x.Company)
                .Include(x => x.RouteInfo)
                .Single(x => x.Id.Equals(id));
            // Use absolute value for the distance
            selectedLeg.RouteInfo!.Distance = Math.Abs(selectedLeg.RouteInfo.Distance);
            vm.SelectedLegs.Add(selectedLeg);

            // Now rebuild the output, using the selected leg
            vm = await ApplyFiltering(vm, false);
            return View(nameof(Create), vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLastLeg(CreateReservationViewModel vm)
        {
            // rebuild the previously selected legs from the selected IDs
            await RebuildSelectedLegs(vm);

            // Get the reference to the new selected leg
            var lastLeg = vm.SelectedLegs.LastOrDefault();
            if (lastLeg != null)
                vm.SelectedLegs.Remove(lastLeg);

            // Now rebuild the output
            vm = await ApplyFiltering(vm, false);
            return View(nameof(Create), vm);
        }

        private async Task RebuildSelectedLegs(CreateReservationViewModel vm)
        {
            if (!String.IsNullOrWhiteSpace(vm.SelectedLegIds))
            {
                var ids = vm.SelectedLegIds.Split(",").Select(x => Guid.Parse(x)).ToList();
                // using flight start as sort order, instead of sorting by the original Id sequence, because it should be the same
                vm.SelectedLegs = await _context.Providers
                    .Include(x => x.Company)
                    .Include(x => x.RouteInfo)
                    .Where(x => ids.Contains(x.Id))
                    .OrderBy(x => x.FlightStart)
                    .ToListAsync();
                // Use absolute value for the distance
                foreach (var leg in vm.SelectedLegs)
                {
                    leg.RouteInfo!.Distance = Math.Abs(leg.RouteInfo.Distance);
                }
            }
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateReservationViewModel vm)
        {
            // Custom validation first
            if (String.IsNullOrWhiteSpace(vm.SelectedLegIds))
                ModelState.TryAddModelError(nameof(vm.SelectedLegIds), "You must select a flight first");
            //else
            //    ModelState.TryAddModelError(nameof(vm.SelectedLegIds), String.Empty);

            if (ModelState.IsValid)
            {
                // rebuild the previously selected legs from the selected IDs
                await RebuildSelectedLegs(vm);
                // Now rebuild the output, no leg selection here ;)
                vm = await ApplyFiltering(vm, false);

                // Now create the booking
                var reservation = new Reservation()
                {
                    Id = Guid.NewGuid(),
                    PriceListId = vm.SelectedPriceListId,

                    From = vm.From!,
                    FirstName = vm.FirstName,
                    LastName = vm.LastName,

                    To = vm.To,
                    TotalPrice = vm.TotalQuotedPrice!.Value,
                    TotalFlightTime = vm.TotalTravelTime!.Value,
                    Companies = vm.CompanyNames,
                    
                    
                };

                reservation.LayOvers = reservation.From;
            
                foreach (var leg in vm.SelectedLegs)
                {
                    reservation.LayOvers += " - " + leg.RouteInfo!.To;
                    reservation.Routes.Add(new FlightRoute
                    {
                        Id = Guid.NewGuid(),
                        ProviderId = leg.Id,
                    });
                }

                
                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), reservation);
            }


            return View(vm);
        }

        /*private FlightPlan BuildFlightPlan(params Provider[] routes)
        {
            var flightPlan = new FlightPlan
            {
                Legs = routes.Select(x => x.RouteInfo).ToArray()!,
                TotalPrice = routes.Sum(x => x.Price),
                CompanyNames = String.Join(", ", routes.Select(x => x.Company!.Name).Distinct()),
                TotalTravelTime = TimeSpan.Zero
            };
            foreach (var route in routes)
                flightPlan.TotalTravelTime = flightPlan.TotalTravelTime.Add(route.TravelTime);

            return flightPlan;
        }*/

       
        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            var vm = new DetailsDeleteReservationViewModel();
            if (id == null )
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                    .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }
            
            vm.Id = reservation.Id;
            vm.FirstName = reservation.FirstName;
            vm.LastName = reservation.LastName;
            vm.From = reservation.From;
            if (reservation.To != null) vm.To = reservation.To;
            vm.TotalPrice = reservation.TotalPrice.ToString(CultureInfo.CurrentCulture);
            vm.TotalFlightTime = reservation.TotalFlightTimeFormatted;
            if (reservation.LayOvers != null) vm.LayOvers = reservation.LayOvers;
            vm.CompanyNames = reservation.Companies;
            return View(vm);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
