// <copyright file="EventPlannerContext.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace EventPlannerWeb.Data
{
    using DAL.Annotation;
    using EventPlannerWeb.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using System.Collections.Generic;

    public class EventPlannerContext : DbContext
    {
        //private readonly IConfiguration? _configuration;

        //public EventPlannerContext(IConfiguration configuration)
        //{
        //    _configuration = configuration;
        //}

        public EventPlannerContext(DbContextOptions<EventPlannerContext> options)
            : base(options)
        {
            //this.Database.Migrate();
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("EventPlannerDBCon"));
        //    }
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var annotationCollection = new List<IEntityAnnotation>
            {
                new UserAnnotation(modelBuilder),
                new RecipeAnnotation(modelBuilder),
                new GuestAnnotation(modelBuilder),
                new EventGuestAnnotation(modelBuilder),
                new EventRecipeAnnotaion(modelBuilder),
                new EventAnnotation(modelBuilder),
                new IngredientRecipeAnnotation(modelBuilder),
                new IngredientAnnotation(modelBuilder),
            };
            foreach (var annotation in annotationCollection)
            {
                annotation.Annotate();
            }
        }

        public DbSet<User> User { get; set; }

        public DbSet<Recipe> Recipe { get; set; }

        public DbSet<Guest> Guest { get; set; }

        public virtual DbSet<Ingredient> Ingredient { get; set; }

        public DbSet<IngredientRecipe> IngredientRecipe { get; set; }

        public DbSet<Event> Event { get; set; }

        public DbSet<EventGuest> EventGuest { get; set; }

        public DbSet<EventRecipe> EventRecipe { get; set; }
    }
}
