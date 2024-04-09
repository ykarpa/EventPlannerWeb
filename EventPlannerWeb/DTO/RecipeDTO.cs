using EventPlannerWeb.Models;

namespace EventPlannerWeb.DTO
{
    public class RecipeDTO
    {
        public Recipe Recipe { get; set; }

        public List<string> Ingredients { get; set; }

        public List<int> IngredientsAmount { get; set; }
    }
}
