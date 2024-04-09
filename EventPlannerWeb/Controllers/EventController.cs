using EventPlannerWeb.Data;
using EventPlannerWeb.DTO;
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
        public async Task<ActionResult<IEnumerable<EventDTO>>> EventList()
        {
            //var eventguest = await _context.Event
            //    .Include(e => e.EventGuests)
            //    .ThenInclude(e => e.Guest).Select(e => e.EventGuests.Select(eg => eg.Guest)).ToListAsync();

            //var eventrecipe = await _context.Event
            //    .Include(e => e.EventRecipes)
            //    .ThenInclude(e => e.Recipe).Select(e => e.EventRecipes.Select(eg => eg.Recipe)).ToListAsync();

            var eventWithGuestsAndRecipes = await _context.Event
                .Include(e => e.EventGuests)
                .ThenInclude(e => e.Guest)
                .Include(e => e.EventRecipes)
                .ThenInclude(e => e.Recipe)
                .ToListAsync();


            var eventDTOs =eventWithGuestsAndRecipes.Select(eventWithGuestsAndRecipes => new EventDTO
            {
                Event = eventWithGuestsAndRecipes,
                Recipes = eventWithGuestsAndRecipes.EventRecipes.Select(er => er.Recipe).ToList(),
                Guests = eventWithGuestsAndRecipes.EventGuests.Select(eg => eg.Guest).ToList()
            }).ToList();


            return Ok(eventDTOs);
            //return View(eventList);
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
        public async Task<ActionResult> AddEvent(EventDTO evenDTO)
        {
            //even.EventGuests
            if (ModelState.IsValid)
            {
                var even = evenDTO.Event;
                await _context.Event.AddAsync(even);
                await _context.SaveChangesAsync();

                foreach(var guest in evenDTO.Guests)
                {
                    var guestNameSurname = $"{guest.Name} {guest.Surname}";
                    int guestId = await GetGuestIdByNameAsync(guestNameSurname);
                    if (guestId != -1)
                    {
                        var eventGuest = new EventGuest { EventId = even.EventId, GuestId = guestId };
                        await _context.EventGuest.AddAsync(eventGuest);
                        //await _context.SaveChangesAsync();
                    }
                    else
                    {
                        return BadRequest("Don't exist");
                    }
                }
                foreach (var recipe in evenDTO.Recipes)
                {
                    int recipeId = await GetRecipeIdByNameAsync(recipe.Name);
                    if (recipeId != -1)
                    {
                        var eventRecipe = new EventRecipe { EventId = even.EventId, RecipeId = recipeId };
                        await _context.EventRecipe.AddAsync(eventRecipe);
                        //await _context.SaveChangesAsync();
                    }
                }

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

        public async Task<int> GetGuestIdByNameAsync(string guestName)
        {
            var gues = guestName.Split(' ');
            var guest = await _context.Guest
                .FirstOrDefaultAsync(a => a.Name == gues[0] && a.Surname == gues[1]);

            if (guest != null)
            {
                return guest.GuestId;
            }
            else
            {
                // Handle the case where the author is not found
                return -1; // Or throw an exception, return null, etc.
            }
        }

        public async Task<int> GetRecipeIdByNameAsync(string recipeName)
        {
            var recipe = await _context.Recipe
                .FirstOrDefaultAsync(a => a.Name == recipeName);

            if (recipe != null)
            {
                return recipe.RecipeId;
            }
            else
            {
                // Handle the case where the author is not found
                return -1; // Or throw an exception, return null, etc.
            }
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
