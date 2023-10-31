using App.DAL.EF;
using App.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class ProvidersController : Controller
    {
        private readonly AppDbContext _context;

        public ProvidersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Providers
        public async Task<IActionResult> Index()
        {
            var vm = new IndexProviderViewModel();
            vm.SelectedPriceListId = await _context.PriceLists
                .AsQueryable()
                    .OrderByDescending(x => x.ValidUntil)
                    .Select(x => x.Id)
                    .FirstOrDefaultAsync();
            vm = await ApplyFiltering(vm);
            return View(vm);
        }

        // GET: Providers/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var provider = await _context.Providers
                .Include(p => p.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (provider == null)
            {
                return NotFound();
            }

            return View(provider);
        }


        public async Task<IActionResult> FilteringByCompanyNameView([FromForm] string search, IndexProviderViewModel vm)
        {
            vm.CompanySearch = search;
            vm = await ApplyFiltering(vm);
            return View(nameof(Index), vm);
        }

        public async Task<IndexProviderViewModel> ApplyFiltering(IndexProviderViewModel vm)
        {
            var query = _context.Providers
                .Include(p => p.RouteInfo)
                .Include(p => p.Company)
                .AsQueryable();
            
            if (!String.IsNullOrWhiteSpace(vm.SelectedItem?.Value) && Guid.TryParse(vm.SelectedItem.Value, out Guid selectedId))
                query = query.Where(p => p.RouteInfo!.PriceListId.Equals(selectedId));
            
            if (!String.IsNullOrWhiteSpace(vm.CompanySearch))
                query = query.Where(p => 
                    p.Company!.Name.Contains(vm.CompanySearch.ToLower()));

            switch (vm.SelectedSortOption?.Value ?? String.Empty)
            {
                case nameof(Provider.Price):
                    query = query.OrderBy(x => x.Price);
                    break;
                case nameof(Provider.TravelTime):
                    query = query.OrderBy(x => x.TravelTime);
                    break;
                case nameof(Provider.RouteInfo.Distance):
                    query = query.OrderBy(x => x.RouteInfo!.Distance);
                    break;
            }
            
            vm.Providers = await query.ToListAsync();
      
            return vm;
        }
        
        public  Task<IActionResult> GettingPriceListsForView()
        {
            //var res = await GettingAll();
            return Task.FromResult<IActionResult>(View(nameof(Index))) ;
        }

        public async Task<List<SelectListItem>> GettingAll()
        {
            return await _context.Providers.Include(p => p.RouteInfo).
                ThenInclude(p => p!.PriceList)
                .OrderByDescending(p => p.RouteInfo!.PriceList!.ValidUntil)
                .Select(p => new SelectListItem(p.RouteInfo!.PriceList!.ValidUntil.ToString(), p.Id.ToString()))
                .ToListAsync();
        }
        
    }
}
