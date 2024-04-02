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
        public async Task<ActionResult<IEnumerable<Guest>>> GetGuests()
        {
            return await _context.Guest.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Guest>> GetGuest(int id)
        {
            var guest = await _context.Guest.FirstOrDefaultAsync(g => g.GuestId == id);

            if (guest == default) return NotFound();

            return guest;
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


        public IActionResult Index()
        {
            return View();
        }

        private IEnumerable<string> GetModelValidationErrors()
        {
            return ModelState.Values.SelectMany(v => v.Errors)
                .Select(v => v.ErrorMessage);
        }
    }
}

