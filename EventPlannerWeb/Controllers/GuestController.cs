using EventPlannerWeb.Data;
using EventPlannerWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

            return View("GuestPage", guest); 
        }

        [HttpGet("AddGuest")]
        public async Task<IActionResult> AddGuestPage()
        {

            var guests = await _context.Guest
               .Select(i => new SelectListItem { Value = i.GuestId.ToString(), Text = i.Name })
               .ToListAsync();

            ViewBag.Guests = guests;

            return View("AddGuest");
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

        private IEnumerable<string> GetModelValidationErrors()
        {
            return ModelState.Values.SelectMany(v => v.Errors)
                .Select(v => v.ErrorMessage);
        }
    }
}

