using System.Diagnostics;
using App.DAL.EF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.ViewModels;

namespace WebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _context;
    public HomeController(ILogger<HomeController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    
    public async Task<IActionResult> Lookup(IndexHomeViewModel vm)
    {
        var record =
            await _context.Reservations.FirstOrDefaultAsync(x => x.Id == vm.Id && x.LastName == vm.LookupLastName);
        if (record == null)
        {
            ModelState.AddModelError(nameof(vm.Id), "Not Found");
        }
        else
        {
            return Redirect($"~/Reservations/Details/{ vm.Id }");
        }

        return View(nameof(Index), vm);
    }
}