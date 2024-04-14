using EventPlannerWeb.Data;
using EventPlannerWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventPlannerWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GuestController : Controller
    {

        private readonly EventPlannerContext _context;

        public GuestController(EventPlannerContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GuestList()
        {
            var guestList = await _context.Guest.ToListAsync();
            return View(guestList);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GuestPage(int id)
        {
            var guest = await _context.Guest.FirstOrDefaultAsync(g => g.GuestId == id);

            if (guest == null) return NotFound();

            return View(guest); 
        }

        [HttpPost]
        public async Task<ActionResult> AddGuest(Guest guest)
        {
            if (ModelState.IsValid)
            {
                await _context.Guest.AddAsync(guest);
                await _context.SaveChangesAsync();

                return Ok();
            }
            var message = GetModelValidationErrors();

            return BadRequest(message);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteGuest(int id)
        {
            var guest = await _context.Guest.FirstOrDefaultAsync(g => g.GuestId == id);
            if (guest == default) return NotFound();

            _context.Guest.Remove(guest);

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> UpdateGuest(Guest guest)
        {
            if (guest.GuestId == default || !GuestExists(guest.GuestId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            _context.Guest.Update(guest);
            await _context.SaveChangesAsync();
            return Ok();
        }

        private bool GuestExists(int id)
        {
            return _context.Guest.Any(e => e.GuestId == id);
        }

        private IEnumerable<string> GetModelValidationErrors()
        {
            return ModelState.Values.SelectMany(v => v.Errors)
                .Select(v => v.ErrorMessage);
        }
    }
}
