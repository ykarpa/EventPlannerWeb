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
        [HttpGet("{id}")]
        public async Task<IActionResult> RecipePage(int id)
        {
            var rec = await _context.Recipe
                .FirstOrDefaultAsync(e => e.RecipeId == id);

            if (rec == null) return NotFound();

            return View("RecipePage", rec);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recipe>>> RecipeList()
        {
            var recipeDTOs = await _context.Recipe
                .Include(r => r.IngredientsRecipe)
                .ThenInclude(ir => ir.Ingredient)
                .Select(r => new RecipeDTO
                {
                    Recipe = r,
                    Ingredients = r.IngredientsRecipe.Select(ir => ir.Ingredient.Name).ToList(),
                    IngredientsAmount = r.IngredientsRecipe.Select(ir => ir.Amount).ToList(),
                }).ToListAsync();
            return View(recipeDTOs);
        }

        //[HttpGet]
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

        [HttpGet("AddRecipe")]
        public async Task<IActionResult> AddRecipePage()
        {
            var ingredients = await _context.Ingredient
                .Select(i => new SelectListItem { Value = i.IngredientId.ToString(), Text = i.Name })
                .ToListAsync();

            ViewBag.Ingredients = ingredients;

            return View("AddRecipe");
        }

        [HttpPost]
        public async Task<ActionResult> AddRecipe(RecipeDTO recipeDTO)
        {
            recipeDTO.Recipe.CreatedDate = DateTime.UtcNow;
            if (ModelState.IsValid)
            {
                var recipe = recipeDTO.Recipe;
                await _context.Recipe.AddAsync(recipe);
                await _context.SaveChangesAsync();

                for (int i = 0; i < recipeDTO.Ingredients.Count; ++i)
                {
                    int ingId = await GetIngredientIdByNameAsync(recipeDTO.Ingredients[i]);
                    if (ingId != -1)
                    {
                        var ingredientRecipe = new IngredientRecipe { Amount = recipeDTO.IngredientsAmount[i], IngredientId = ingId, RecipeId = recipe.RecipeId };
                        await _context.IngredientRecipe.AddAsync(ingredientRecipe);
                        await _context.SaveChangesAsync();
                    }
                }

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

        [HttpPut]
        public async Task<ActionResult> UpdateRecipe(RecipeDTO recipeDTO)
        {
            if (recipeDTO.Recipe.RecipeId == default || !RecipeExists(recipeDTO.Recipe.RecipeId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            recipeDTO.Recipe.ModifiedDate = DateTime.UtcNow;

            _context.Recipe.Update(recipeDTO.Recipe);

            var recipeIngredientToDelete = GetAllRecipeIngredients(recipeDTO.Recipe.RecipeId);

            for (int i = 0; i < recipeDTO.Ingredients.Count; ++i)
            {
                int id = await GetIngredientIdByNameAsync(recipeDTO.Ingredients[i]);
                if (await RecipeIngredientExists(recipeDTO.Recipe.RecipeId, id) == false && id != -1)
                {
                    var ingredientRecipe = new IngredientRecipe { Amount = recipeDTO.IngredientsAmount[i], IngredientId = id, RecipeId = recipeDTO.Recipe.RecipeId };
                    await _context.IngredientRecipe.AddAsync(ingredientRecipe);
                }
                recipeIngredientToDelete.Remove(id);
            }
            foreach (var ingredientToDelete in recipeIngredientToDelete)
            {
                var ingredientRecipe = await _context.IngredientRecipe.FirstOrDefaultAsync(e => e.RecipeId == recipeDTO.Recipe.RecipeId && e.IngredientId == ingredientToDelete);
                _context.IngredientRecipe.Remove(ingredientRecipe);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        private bool RecipeExists(int id)
        {
            return _context.Recipe.Any(e => e.RecipeId == id);
        }

        private async Task<bool> RecipeIngredientExists(int recipeId, int ingredientId)
        {
            return _context.IngredientRecipe.Any(ri => ri.RecipeId == recipeId && ri.IngredientId == ingredientId);
        }

        private List<int> GetAllRecipeIngredients(int recipeId)
        {
            var recipeIngredients = _context.IngredientRecipe.Where(er => er.RecipeId == recipeId).ToList();
            var ingredients = new List<int>();
            foreach (var recipeIngredient in recipeIngredients)
            {
                ingredients.Add(recipeIngredient.IngredientId);
            }
            return ingredients;
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
