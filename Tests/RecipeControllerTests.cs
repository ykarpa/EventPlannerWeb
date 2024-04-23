using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventPlannerWeb.Controllers;
using EventPlannerWeb.Data;
using EventPlannerWeb.DTO;
using EventPlannerWeb.Models;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Tests
{

    public class RecipeControllerTests
    {
        private readonly DbContextOptions<EventPlannerContext> _options;

        public RecipeControllerTests()
        {
            _options = new DbContextOptionsBuilder<EventPlannerContext>()
                .UseInMemoryDatabase(databaseName: "Test_EventPlannerDB")
                .Options;
        }

        [Fact]
        public async Task AddRecipe_InvalidModel_ReturnsBadRequestWithValidationErrors()
        {
            // Arrange
            var invalidRecipeDTO = new RecipeDTO
            {
                Recipe = new Recipe { Name = null, Description = "Test Description" },
                Ingredients = new List<string> { "Ingredient1", "Ingredient2" },
                IngredientsAmount = new List<int> { 1, 2 }
            };

            var mockContext = new Mock<EventPlannerContext>(new DbContextOptions<EventPlannerContext>());
            var controller = new RecipeController(mockContext.Object);
            controller.ModelState.AddModelError("Name", "The Name field is required."); // Simulate ModelState error

            // Act
            var result = await controller.AddRecipe(invalidRecipeDTO);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorMessage = Assert.IsAssignableFrom<IEnumerable<string>>(actionResult.Value);
            Assert.Contains("The Name field is required.", errorMessage);
        }
        [Fact]
        public async Task UpdateRecipe_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var invalidRecipeDTO = new RecipeDTO
            {
                Recipe = new Recipe { Name = null, Description = "Test Description" },
                Ingredients = new List<string> { "Ingredient1", "Ingredient2" },
                IngredientsAmount = new List<int> { 1, 2 }
            };

            var mockContext = new Mock<EventPlannerContext>(new DbContextOptions<EventPlannerContext>());
            var controller = new RecipeController(mockContext.Object);
            controller.ModelState.AddModelError("Name", "The Name field is required."); // Simulate ModelState error

            // Act
            var result = await controller.UpdateRecipe(invalidRecipeDTO);

            // Assert
            Assert.IsAssignableFrom<BadRequestResult>(result);
        }
        [Fact]
        public async Task DeleteRecipe_ReturnsNotFound_WhenRecipeNotFound()
        {
            // Arrange
            using (var context = new EventPlannerContext(_options))
            {
                // Initialize database with test data
                context.Recipe.Add(new Recipe { Name = "name", Price = 10 });
                await context.SaveChangesAsync();
            }

            using (var context = new EventPlannerContext(_options))
            {
                var controller = new RecipeController(context);

                // Act
                var result = await controller.DeleteRecipe(2); // Assuming ingredient with ID 2 doesn't exist

                // Assert
                Assert.IsType<NotFoundResult>(result);
            }
        }

        [Fact]
        public async Task DeleteRecipe_ReturnsOkResult_WhenRecipeDeleted()
        {
            // Arrange
            using (var context = new EventPlannerContext(_options))
            {
                // Initialize database with test data
                var ingrData = new Recipe { RecipeId = 5, Name = "test", Price = 10 };
                context.Recipe.Add(ingrData);
                await context.SaveChangesAsync();
            }

            using (var context = new EventPlannerContext(_options))
            {
                var controller = new RecipeController(context);

                // Act
                var result = await controller.DeleteRecipe(5);

                // Assert
                var okResult = Assert.IsType<OkResult>(result);
                Assert.Equal(200, okResult.StatusCode);
            }
        }
        [Fact]
        public async Task RecipeList_ReturnsViewWithNoRecipeDTOs_WhenNoRecipesExist()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<EventPlannerContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDb")
                .Options;

            using (var context = new EventPlannerContext(options))
            {
                // Initialize the database with some data if needed
                // Add any seed data if needed
            }

            using (var context = new EventPlannerContext(options))
            {
                var controller = new RecipeController(context);

                // Act
                var result = await controller.RecipeList();

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsAssignableFrom<IEnumerable<RecipeDTO>>(viewResult.Model);
                Assert.Empty(model); // Assert that the model is empty
            }
        }
        [Fact]
        public async Task UpdateRecipePage_ReturnsNotFound_WhenRecipeNotFound()
        {
            // Arrange
            var nonExistingRecipeId = 999; // ID of a non-existing recipe

            using (var context = new EventPlannerContext(_options))
            {
                // No recipe is added to the database
                var controller = new RecipeController(context);

                // Act
                var result = await controller.UpdateRecipePage(nonExistingRecipeId);

                // Assert
                Assert.IsType<NotFoundResult>(result);
            }
        }




    }

}


