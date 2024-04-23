using EventPlannerWeb.Controllers;
using EventPlannerWeb.Data;
using EventPlannerWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Tests
{
    public class UserControllerTests
    {
        private readonly DbContextOptions<EventPlannerContext> _options;

        public UserControllerTests()
        {
            _options = new DbContextOptionsBuilder<EventPlannerContext>()
                .UseInMemoryDatabase(databaseName: "Test_EventPlannerDB")
                .Options;
        }

        [Fact]
        public async Task UpdateUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<EventPlannerContext>()
                .UseInMemoryDatabase(databaseName: "EventPlannerTest")
                .Options;
            var mockContext = new EventPlannerContext(options);
            var controller = new UserController(mockContext);

            // Act
            var user = new User { Id = 1, Name = "Test User" };
            var result = await controller.UpdateUser(user);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public async Task AddUser_InvalidModel_ReturnsBadRequestWithValidationErrors()
        {
            // Arrange
            var invalidUser = new User { Name = null, Email = "test@example.com" }; // Username is required

            var mockContext = new Mock<EventPlannerContext>(new DbContextOptions<EventPlannerContext>());
            var controller = new UserController(mockContext.Object);
            controller.ModelState.AddModelError("Username", "The Username field is required."); // Simulate ModelState error

            // Act
            var result = await controller.AddUser(invalidUser);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorMessage = Assert.IsAssignableFrom<IEnumerable<string>>(actionResult.Value);
            Assert.Contains("The Username field is required.", errorMessage);
        }

        [Fact]
        public async Task DeleteUser_ReturnsNotFound_WhenUserNotFound()
        {
            // Arrange
            using (var context = new EventPlannerContext(_options))
            {
                // Initialize database with test data
                context.User.Add(new User { Id = 1, Name = "Test user", Surname = "Test", Email = "test@example.com", Gender = Gender.Male/*, Password = "123567890"*/, PhoneNumber="0675629289" });
                await context.SaveChangesAsync();
            }

            using (var context = new EventPlannerContext(_options))
            {
                var controller = new UserController(context);

                // Act
                var result = await controller.DeleteUser(2); // Assuming guest with ID 2 doesn't exist

                // Assert
                Assert.IsType<NotFoundResult>(result);
            }
        }

        [Fact]
        public async Task DeleteUser_ReturnsOkResult_WhenUserDeleted()
        {
            // Arrange
            using (var context = new EventPlannerContext(_options))
            {
                // Initialize database with test data
                var userData = new User { Id = 5, Name = "Test user", Surname = "Test", Email = "test@example.com", Gender = Gender.Male, /*Password = "123567890",*/ PhoneNumber = "0675629289" };
                context.User.Add(userData);
                await context.SaveChangesAsync();
            }

            using (var context = new EventPlannerContext(_options))
            {
                var controller = new UserController(context);

                // Act
                var result = await controller.DeleteUser(5);

                // Assert
                var okResult = Assert.IsType<OkResult>(result);
                Assert.Equal(200, okResult.StatusCode);
            }
        }
        [Fact]
        public async Task GetUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var nonExistingUserId = 999; // ID of a non-existing user

            using (var context = new EventPlannerContext(_options))
            {
                // No user is added to the database
                var controller = new UserController(context);

                // Act
                var result = await controller.GetUser(nonExistingUserId);

                // Assert
                Assert.IsType<NotFoundResult>(result.Result);
            }
        }
    }
}