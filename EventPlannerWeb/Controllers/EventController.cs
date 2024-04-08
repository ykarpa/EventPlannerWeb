using EventPlannerWeb.Data;
using EventPlannerWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventPlannerWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : Controller
    {

        private readonly EventPlannerContext _context;

        public EventController(EventPlannerContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> EventList()
        {
            var eventList = await _context.Event.ToListAsync();
            return View(eventList);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> EventPage(int id)
        {
            var even = await _context.Event
                .Include(e => e.EventGuests) 
                .Include(e => e.EventRecipes) 
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (even == null) return NotFound();

            return View("EventPage", even); 
        }


        [HttpPost]
        public async Task<ActionResult> AddEvent(Event even)
        {
            //even.EventGuests
            if (ModelState.IsValid)
            {
                await _context.Event.AddAsync(even);
                await _context.SaveChangesAsync();

                return Ok();
            }
            var message = GetModelValidationErrors();

            return BadRequest(message);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEvent(int id)
        {
            var even = await _context.Event.FirstOrDefaultAsync(e => e.EventId == id);
            if (even == default) return NotFound();

            _context.Event.Remove(even);

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
