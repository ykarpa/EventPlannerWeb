// <copyright file="EventPlannerContext.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace EventPlannerWeb.Data
{
    using DAL.Annotation;
    using EventPlannerWeb.Models;
    using Library_kursova.Entities;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using System.Collections.Generic;

    public class EventPlannerContext : IdentityDbContext<User, AppRole, int,
        IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {

        public EventPlannerContext(DbContextOptions<EventPlannerContext> options)
            : base(options)
        {
        }

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
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AppRole>().HasMany(u => u.UserRoles).WithOne(u => u.Role).HasForeignKey(u => u.RoleId).IsRequired();
        }

        public DbSet<User> User { get; set; }

        public virtual DbSet<Recipe> Recipe { get; set; }

        public virtual DbSet<Guest> Guest { get; set; }

        public virtual DbSet<Ingredient> Ingredient { get; set; }

        public DbSet<IngredientRecipe> IngredientRecipe { get; set; }

        public virtual DbSet<Event> Event { get; set; }

        public DbSet<EventGuest> EventGuest { get; set; }

        public DbSet<EventRecipe> EventRecipe { get; set; }
    }
}
