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
    public class RecipeController : Controller
    {

        private readonly EventPlannerContext _context;

        public RecipeController(EventPlannerContext context)
        {
            _context = context;
        }

        //[HttpGet]
        //public async Task<IActionResult> RecipeList()
        //{
        //    var recipeList = await _context.Recipe.ToListAsync(); 
        //    return View(recipeList); 
        //}

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipes()
        //{
        //    var recipeDTOs = await _context.Recipe
        //        .Include(r => r.IngredientsRecipe)
        //        .ThenInclude(ir => ir.Ingredient)
        //        .Select(r => new RecipeDTO
        //        {
        //            Recipe = r,
        //            Ingredients = r.IngredientsRecipe.Select(ir => ir.Ingredient.Name).ToList(),
        //            IngredientsAmount = r.IngredientsRecipe.Select(ir => ir.Amount).ToList(),
        //        }).ToListAsync();
        //    return Ok(recipeDTOs);
        //}

        //[HttpGet("{id}")]
        //public async Task<ActionResult<RecipeDTO>> GetRecipe(int id)
        //{
        //    var recipeDTO = await _context.Recipe
        //        .Where(r => r.RecipeId == id)
        //        .Include(r => r.IngredientsRecipe)
        //        .ThenInclude(ir => ir.Ingredient)
        //        .Select(r => new RecipeDTO
        //        {
        //            Recipe = r,
        //            Ingredients = r.IngredientsRecipe.Select(ir => ir.Ingredient.Name).ToList(),
        //            IngredientsAmount = r.IngredientsRecipe.Select(ir => ir.Amount).ToList(),
        //        }).FirstOrDefaultAsync();

        //    if (recipeDTO == default) return NotFound();

        //    return recipeDTO;
        //}

        [HttpGet("{id}")]
        public async Task<IActionResult> RecipePage(int id)
        {
            var rec = await _context.Recipe
                .FirstOrDefaultAsync(e => e.RecipeId == id);

            if (rec == null) return NotFound();

            return View("RecipePage", rec);
        }
        [HttpGet("AddRecipePage")]
        public async Task<IActionResult> AddRecipePage()
        {
            var ingredients = await _context.Ingredient
                .Select(i => new SelectListItem { Value = i.IngredientId.ToString(), Text = i.Name })
                .ToListAsync();

            ViewBag.Ingredients = ingredients;

            return View("AddRecipe");
        }

        [HttpPost("AddRecipe")]
        public async Task<ActionResult> AddRecipe([FromBody] RecipeDTO recipeWithIngredientsDTO)
        {
            RecipeDTO recipeDTO = recipeWithIngredientsDTO;
            List<string> selectedIngredients = recipeWithIngredientsDTO.Ingredients;

            if (ModelState.IsValid)
            {
                var recipe = recipeDTO.Recipe;

                // Add recipe to the database
                await _context.Recipe.AddAsync(recipe);
                await _context.SaveChangesAsync();

                // Add ingredients to the recipe
                foreach (var ingredientIdString in selectedIngredients)
                {
                    if (int.TryParse(ingredientIdString, out int ingredientId))
                    {
                        var ingredientRecipe = new IngredientRecipe { IngredientId = ingredientId, RecipeId = recipe.RecipeId };
                        await _context.IngredientRecipe.AddAsync(ingredientRecipe);
                    }
                    else
                    {
                        // Handle invalid ingredientId
                        return BadRequest($"Invalid ingredient ID: {ingredientIdString}");
                    }
                }
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

        public async Task<int> GetIngredientIdByNameAsync(string ingredientName)
        {
            var ingredient = await _context.Ingredient
                .FirstOrDefaultAsync(a => a.Name == ingredientName);

            if (ingredient != null)
            {
                return ingredient.IngredientId;
            }
            else
            {
                // Handle the case where the author is not found
                return -1; // Or throw an exception, return null, etc.
            }
        }

        private IEnumerable<string> GetModelValidationErrors()
        {
            return ModelState.Values.SelectMany(v => v.Errors)
                .Select(v => v.ErrorMessage);
        }
    }
}

