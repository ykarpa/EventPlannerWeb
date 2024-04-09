using EventPlannerWeb.Data;
using EventPlannerWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventPlannerWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IngredientController : Controller
    {
        private readonly EventPlannerContext _context;

        public IngredientController(EventPlannerContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ingredient>>> IngredientList()
        {
            var ingredientList = await _context.Ingredient.ToListAsync();

            return ingredientList;
            //return View(ingredientList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Ingredient>> IngredientPage(int id)
        {
            var ingredient = await _context.Ingredient
                .FirstOrDefaultAsync(e => e.IngredientId == id);

            if (ingredient == null) return NotFound();

            return ingredient;

            //return View("IngredientPage", ingredient);
        }


        [HttpPost]
        public async Task<ActionResult> AddIngredient(Ingredient ingredient)
        {
            //even.EventGuests
            if (ModelState.IsValid)
            {
                await _context.Ingredient.AddAsync(ingredient);
                await _context.SaveChangesAsync();

                return Ok();
            }
            var message = GetModelValidationErrors();

            return BadRequest(message);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteIngredient(int id)
        {
            var ingredient = await _context.Ingredient.FirstOrDefaultAsync(e => e.IngredientId == id);
            if (ingredient == default) return NotFound();

            _context.Ingredient.Remove(ingredient);

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
