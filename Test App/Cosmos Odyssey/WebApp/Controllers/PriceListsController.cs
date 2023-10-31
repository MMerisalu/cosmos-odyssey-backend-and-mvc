using App.DAL.EF;
using App.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Helpers;

namespace WebApp.Controllers
{
    public class PriceListsController : Controller
    {
      

        //private readonly Uri _baseAddress = new Uri();
        private readonly HttpClient _client;
        private readonly AppDbContext _context;


        public PriceListsController(AppDbContext context)
        {
            _context = context;
            _client = new HttpClient();
            //_client.BaseAddress = _baseAddress;
        }

        // GET: PriceLists
        public async Task<IActionResult> Index()
        {
            await PriceListHelper.RefreshPriceList(_context, _client);
            // I still use Take(15) here to allow for the case where there are multiple concurrent users
            var  priceLists = _context.PriceLists
                    .OrderByDescending(p => p.ValidUntil)
                    .Take(15)
                    .ToList();
            return View(priceLists);
        }

        // GET: PriceLists/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priceList = await _context.PriceLists
                .FirstOrDefaultAsync(m => m.Id == id);
            if (priceList == null)
            {
                return NotFound();
            }

            return View(priceList);
        }

        // GET: PriceLists/Create
        /*public IActionResult Create()
        {
            return View();
        }
        */

        // POST: PriceLists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ValidUntil,Id")] PriceList priceList)
        {
            if (ModelState.IsValid)
            {
                priceList.Id = Guid.NewGuid();
                _context.Add(priceList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(priceList);
        }*/

        // GET: PriceLists/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priceList = await _context.PriceLists.FindAsync(id);
            if (priceList == null)
            {
                return NotFound();
            }

            return View(priceList);
        }

        // POST: PriceLists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ValidUntil,Id")] PriceList priceList)
        {
            if (id != priceList.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(priceList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PriceListExists(priceList.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(priceList);
        }

        // GET: PriceLists/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priceList = await _context.PriceLists
                .FirstOrDefaultAsync(m => m.Id == id);
            if (priceList == null)
            {
                return NotFound();
            }

            return View(priceList);
        }

        // POST: PriceLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var priceList = await _context.PriceLists.FindAsync(id);
            if (priceList != null)
            {
                _context.PriceLists.Remove(priceList);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PriceListExists(Guid id)
        {
            return (_context.PriceLists.Any(e => e.Id == id));
        }

        public Task<IActionResult> RefreshPriceListAction()
        {
            //var priceLists = await PriceListHelper.RefreshPriceList(_context, _client);
            return Task.FromResult<IActionResult>(RedirectToAction(nameof(Index)));
        }
        


    }
}
