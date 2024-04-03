// <copyright file="Recipe.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Text.Json.Serialization;

namespace EventPlannerWeb.Models
{
    public class Recipe
    {
        public int RecipeId { get; set; }

        public string Name { get; set; }

        public int Calories { get; set; }

        public int CookingTime { get; set; }

        public string Description { get; set; }

        [JsonIgnore]
        public ICollection<IngredientRecipe>? IngredientsRecipe { get; set; }

        [JsonIgnore]
        public ICollection<EventRecipe>? RecipeEvents { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string? RecipeImageName { get; set; }

        public double Price { get; set; }
    }
}
