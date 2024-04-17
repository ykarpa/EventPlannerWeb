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
    



    }
    
}
