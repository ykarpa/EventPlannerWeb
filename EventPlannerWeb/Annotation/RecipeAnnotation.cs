namespace DAL.Annotation
{
    using EventPlannerWeb.Models;
    using Microsoft.EntityFrameworkCore;

    internal class RecipeAnnotation : BaseEntityAnnotation<Recipe>
    {
        internal RecipeAnnotation(ModelBuilder builder)
            : base(builder)
        {
        }

        public override void Annotate()
        {
            this.ModelBuilder.HasKey(u => u.RecipeId);
            this.ModelBuilder.Property(u => u.RecipeId).ValueGeneratedOnAdd().UseIdentityColumn().HasColumnName("recipe_id");
            this.ModelBuilder.Property(u => u.Name).IsRequired().HasMaxLength(20).HasColumnName("name");
            this.ModelBuilder.Property(u => u.Calories).IsRequired().HasColumnName("calories");
            this.ModelBuilder.Property(u => u.CookingTime).IsRequired().HasColumnName("cooking_time");
            this.ModelBuilder.Property(u => u.CreatedDate).IsRequired(false).HasColumnName("created_date");
            this.ModelBuilder.Property(u => u.ModifiedDate).HasColumnName("modified_date");
            this.ModelBuilder.Property(u => u.Description).IsRequired(false).HasColumnName("description");
            this.ModelBuilder.Property(u => u.RecipeImageName).HasColumnName("recipe_image").IsRequired(false);
            this.ModelBuilder.Property(u => u.Price).IsRequired().HasColumnName("price");
        }
    }
}
