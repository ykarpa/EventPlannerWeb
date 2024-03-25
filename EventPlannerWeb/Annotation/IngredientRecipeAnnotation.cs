using EventPlannerWeb.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Annotation
{
    internal class IngredientRecipeAnnotation : BaseEntityAnnotation<IngredientRecipe>
    {
        internal IngredientRecipeAnnotation(ModelBuilder builder)
            : base(builder)
        {
        }

        public override void Annotate()
        {
            this.ModelBuilder.HasKey(u => u.IngredientRecipeId);
            this.ModelBuilder.Property(u => u.IngredientRecipeId).ValueGeneratedOnAdd().UseIdentityColumn().HasColumnName("ingredient_recipe_id");
            this.ModelBuilder.HasOne(u => u.Ingredient).WithMany(u => u.RecipesIngerdient).HasForeignKey(u => u.IngredientId);
            this.ModelBuilder.HasOne(u => u.Recipe).WithMany(u => u.IngredientsRecipe).HasForeignKey(u => u.RecipeId);
        }
    }
}