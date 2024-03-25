// <copyright file="Ingredient.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace EventPlannerWeb.Models
{
    public class Ingredient
    {
        public int IngredientId { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public ICollection<IngredientRecipe> RecipesIngerdient { get; set; }
    }
}
