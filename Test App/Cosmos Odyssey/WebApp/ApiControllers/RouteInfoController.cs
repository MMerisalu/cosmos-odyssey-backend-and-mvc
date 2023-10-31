using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain;

namespace WebApp.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RouteInfoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RouteInfoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/RouteInfo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RouteInfo>>> GetRouteInfos()
        {
          if (_context.RouteInfos == null)
          {
              return NotFound();
          }
            return await _context.RouteInfos.ToListAsync();
        }

        // GET: api/RouteInfo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RouteInfo>> GetRouteInfo(Guid id)
        {
          if (_context.RouteInfos == null)
          {
              return NotFound();
          }
            var routeInfo = await _context.RouteInfos.FindAsync(id);

            if (routeInfo == null)
            {
                return NotFound();
            }

            return routeInfo;
        }

        // PUT: api/RouteInfo/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRouteInfo(Guid id, RouteInfo routeInfo)
        {
            if (id != routeInfo.Id)
            {
                return BadRequest();
            }

            _context.Entry(routeInfo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RouteInfoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/RouteInfo
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RouteInfo>> PostRouteInfo(RouteInfo routeInfo)
        {
          if (_context.RouteInfos == null)
          {
              return Problem("Entity set 'AppDbContext.RouteInfos'  is null.");
          }
            _context.RouteInfos.Add(routeInfo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRouteInfo", new { id = routeInfo.Id }, routeInfo);
        }

        // DELETE: api/RouteInfo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRouteInfo(Guid id)
        {
            if (_context.RouteInfos == null)
            {
                return NotFound();
            }
            var routeInfo = await _context.RouteInfos.FindAsync(id);
            if (routeInfo == null)
            {
                return NotFound();
            }

            _context.RouteInfos.Remove(routeInfo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RouteInfoExists(Guid id)
        {
            return (_context.RouteInfos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
