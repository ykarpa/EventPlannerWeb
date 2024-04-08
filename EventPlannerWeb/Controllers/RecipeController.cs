using EventPlannerWeb.Data;
using EventPlannerWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventPlannerWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipeController : Controller
    {

        private readonly EventPlannerContext _context;

        public RecipeController(EventPlannerContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> RecipeList()
        {
            var recipeList = await _context.Recipe.ToListAsync(); 
            return View(recipeList); 
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Recipe>> GetRecipe(int id)
        {
            var recipe = await _context.Recipe.FirstOrDefaultAsync(r => r.RecipeId == id);

            if (recipe == default) return NotFound();

            return recipe;
        }

        [HttpPost]
        public async Task<ActionResult> AddRecipe(Recipe recipe)
        {
            if (ModelState.IsValid)
            {
                await _context.Recipe.AddAsync(recipe);
                await _context.SaveChangesAsync();

                return Ok();
            }
            var message = GetModelValidationErrors();

            return BadRequest(message);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRecipe(int id)
        {
            var recipe = await _context.Recipe.FirstOrDefaultAsync(r => r.RecipeId == id);
            if (recipe == default) return NotFound();

            _context.Recipe.Remove(recipe);

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

