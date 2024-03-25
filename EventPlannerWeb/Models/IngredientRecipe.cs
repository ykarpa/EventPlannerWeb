﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventPlannerWeb.Models
{
    public class IngredientRecipe
    {
        public int IngredientRecipeId { get; set; }

        public int RecipeId { get; set; }

        public int IngredientId { get; set; }

        public Recipe Recipe { get; set; }

        public Ingredient Ingredient { get; set; }

        public int Amount { get; set; }
    }
}
