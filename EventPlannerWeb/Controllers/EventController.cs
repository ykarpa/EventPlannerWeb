using EventPlannerWeb.Data;
using EventPlannerWeb.DTO;
using EventPlannerWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> EventList()
        {
            var even = await _context.Event.ToListAsync();


            return View(even);
            //return View(eventList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EventDTO>> EventPage(int id)
        {
            var eventWithGuestsAndRecipes = await _context.Event
                .Where(e => e.EventId == id)
                .Include(e => e.EventGuests)
                .ThenInclude(e => e.Guest)
                .Include(e => e.EventRecipes)
                .ThenInclude(e => e.Recipe)
                .FirstOrDefaultAsync();

            if (eventWithGuestsAndRecipes == null)
            {
                return NotFound(); // Return 404 Not Found if event is not found
            }

            var eventDTO = new EventDTO
            {
                Event = eventWithGuestsAndRecipes,
                Recipes = eventWithGuestsAndRecipes.EventRecipes?.Select(er => er.Recipe.RecipeId).ToList() ?? new List<int>(),
                Guests = eventWithGuestsAndRecipes.EventGuests?.Select(eg => eg.Guest.GuestId).ToList() ?? new List<int>()
            };

            return eventDTO;
        }



        [HttpGet("AddEvent")]
        public async Task<IActionResult> AddEventPage()
        {
            var recipes = await _context.Recipe
                .Select(i => new SelectListItem { Value = i.RecipeId.ToString(), Text = i.Name })
                .ToListAsync();

            ViewBag.Recipes = recipes;

            var guests = await _context.Guest
                .Select(i => new SelectListItem { Value = i.GuestId.ToString(), Text = i.Name })
                .ToListAsync();

            ViewBag.Guests = guests;

            return View("AddEvent");
        }

        [HttpPost]
        public async Task<ActionResult> AddEvent(EventDTO evenDTO)
        {
            if (evenDTO == null || evenDTO.Event == null)
            {
                return BadRequest("Event data is null");
            }

            if (ModelState.IsValid)
            {
                var even = evenDTO.Event;
                even.CreatedDate = DateTime.UtcNow;
                _context.Event.Add(even); // Removed async operation because EF Core doesn't support async Add
                await _context.SaveChangesAsync();

                foreach (var guestId in evenDTO.Guests)
                {
                    if (await GuestExists(guestId))
                    {
                        var eventGuest = new EventGuest { EventId = even.EventId, GuestId = guestId };
                        await _context.EventGuest.AddAsync(eventGuest);
                    }
                    else
                    {
                        return BadRequest("Guest doesn't exist");
                    }
                }

                foreach (var recipeId in evenDTO.Recipes)
                {
                    if (await RecipeExists(recipeId))
                    {
                        var eventRecipe = new EventRecipe { EventId = even.EventId, RecipeId = recipeId };
                        await _context.EventRecipe.AddAsync(eventRecipe);
                    }
                    else
                    {
                        return BadRequest("Recipe doesn't exist");
                    }
                }

                await _context.SaveChangesAsync();
                return Ok();
            }
            else
            {
                var message = GetModelValidationErrors();

                return BadRequest(message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEvent(int id)
        {
            var even = await _context.Event.FirstOrDefaultAsync(e => e.EventId == id);
            if (even == default) return NotFound();

            try
            {
                _context.Event.Remove(even);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                return StatusCode(500, $"An error occurred while deleting the event: {ex.Message}");
            }
        }


        [HttpPut]
        public async Task<ActionResult> UpdateEvent(EventDTO eventDTO)
        {
            if (eventDTO == null || eventDTO.Event == null)
            {
                return BadRequest("Event data is null");
            }


            if (!ModelState.IsValid)
            {
                var message = GetModelValidationErrors();
                return BadRequest(message);
            }

            eventDTO.Event.ModifiedDate = DateTime.UtcNow;

            _context.Event.Update(eventDTO.Event);

            var eventGuestToDelete = GetAllEventsGuests(eventDTO.Event.EventId);
            var eventRecipeToDelete = GetAllEventsRecipes(eventDTO.Event.EventId);

            foreach (var guestId in eventDTO.Guests)
            {
                if (await GuestExists(guestId) && await GuestEventExists(guestId, eventDTO.Event.EventId) == false)
                {
                    var eventGuest = new EventGuest { EventId = eventDTO.Event.EventId, GuestId = guestId };
                    await _context.EventGuest.AddAsync(eventGuest);
                }
                eventGuestToDelete.Remove(guestId);
            }
            foreach(var guestToDeleteId in eventGuestToDelete)
            {
                var eventGuest = await _context.EventGuest.FirstOrDefaultAsync(e => e.EventId == eventDTO.Event.EventId && e.GuestId == guestToDeleteId);
                _context.EventGuest.Remove(eventGuest);
            }

            foreach (var recipeId in eventDTO.Recipes)
            {
                if (await RecipeExists(recipeId) && await RecipeEventExists(recipeId, eventDTO.Event.EventId) == false)
                {
                    var eventRecipe = new EventRecipe { EventId = eventDTO.Event.EventId, RecipeId = recipeId };
                    await _context.EventRecipe.AddAsync(eventRecipe);
                }
                eventRecipeToDelete.Remove(recipeId);
            }
            foreach (var recipeToDeleteId in eventRecipeToDelete)
            {
                var eventRecipe = await _context.EventRecipe.FirstOrDefaultAsync(e => e.EventId == eventDTO.Event.EventId && e.RecipeId == recipeToDeleteId);
                _context.EventRecipe.Remove(eventRecipe);
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        private bool EventExists(int id)
        {
            return _context.Event.Any(e => e.EventId == id);
        }

        private List<int> GetAllEventsRecipes(int eventId)
        {
            var eventRecipes =  _context.EventRecipe.Where(er => er.EventId == eventId).ToList();
            var recipes = new List<int>();
            foreach (var eventRecipe in eventRecipes)
            {
                recipes.Add(eventRecipe.RecipeId);
            }
            return recipes;
        }

        private List<int> GetAllEventsGuests(int eventId)
        {
            var eventGuests = _context.EventGuest.Where(er => er.EventId == eventId).ToList();
            var guests = new List<int>();
            foreach (var eventGuest in eventGuests)
            {
                guests.Add(eventGuest.GuestId);
            }
            return guests;
        }

        private async Task<bool> RecipeEventExists(int recipeId, int eventId)
        {
            return await _context.EventRecipe.AnyAsync(ri => ri.RecipeId == recipeId && ri.EventId == eventId);
        }

        private async Task<bool> GuestEventExists(int guestId, int eventId)
        {
            return await _context.EventGuest.AnyAsync(ri => ri.GuestId == guestId && ri.EventId == eventId);
        }

        private async Task<bool> GuestExists(int guestId)
        {
            return await _context.Guest.AnyAsync(g => g.GuestId == guestId);
        }

        private async Task<bool> RecipeExists(int recipeId)
        {
            return await _context.Recipe.AnyAsync(g => g.RecipeId == recipeId);
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
