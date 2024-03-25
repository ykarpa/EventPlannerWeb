using DAL.Annotation;
using EventPlannerWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Annotation
{
    internal class IngredientAnnotation : BaseEntityAnnotation<Ingredient>
    {
        internal IngredientAnnotation(ModelBuilder builder)
            : base(builder)
        {
        }

        public override void Annotate()
        {
            this.ModelBuilder.HasKey(u => u.IngredientId);
            this.ModelBuilder.Property(u => u.IngredientId).ValueGeneratedOnAdd().UseIdentityColumn().HasColumnName("ingredient_id");
            this.ModelBuilder.Property(u => u.Name).IsRequired().HasColumnName("name");
            this.ModelBuilder.Property(u => u.Price).IsRequired().HasColumnName("price");
        }
    }
}
