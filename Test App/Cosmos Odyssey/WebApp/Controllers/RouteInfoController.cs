using App.DAL.EF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace WebApp.Controllers
{
    public class RouteInfoController : Controller
    {
        private readonly AppDbContext _context;

        public RouteInfoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: RouteInfo
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.RouteInfos;
            return View(await appDbContext.ToListAsync());
        }

        // GET: RouteInfo/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null )
            {
                return NotFound();
            }

            var routeInfo = await _context.RouteInfos.FirstOrDefaultAsync(m => m.Id == id);
            if (routeInfo == null)
            {
                return NotFound();
            }

            return View(routeInfo);
        }
    }
}
