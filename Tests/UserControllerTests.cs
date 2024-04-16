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
        //[Fact]
        //public async Task GetUser_ReturnsNotFound_WhenUserNotFound()
        //{
        //    // Arrange
        //    var options = new DbContextOptionsBuilder<EventPlannerContext>()
        //        .UseInMemoryDatabase(databaseName: "EventPlannerTest")
        //        .Options;
        //    using (var context = new EventPlannerContext(options))
        //    {
        //        context.User.Add(new User { UserId = 1, Name = "Test User" });
        //        context.SaveChanges();
        //    }

        //    var mockContext = new Mock<EventPlannerContext>(options);
        //    var controller = new UserController(mockContext.Object);

        //    // Act
        //    var result = await controller.GetUser(2);

        //    // Assert
        //    Assert.IsType<NotFoundResult>(result.Result);
        //}

        //[Fact]
        //public async Task AddUser_ReturnsOkResult_WhenModelStateIsValid()
        //{
        //    // Arrange
        //    var options = new DbContextOptionsBuilder<EventPlannerContext>()
        //        .UseInMemoryDatabase(databaseName: "EventPlannerTest")
        //        .Options;
        //    using (var context = new EventPlannerContext(options))
        //    {
        //        var user1 = new User
        //        {
        //            UserId = 1,
        //            Name = "Test User",
        //            Email = "test@example.com", // Provide values for required properties
        //            Gender = (Gender)1,
        //            Password = "password",
        //            PhoneNumber = "1234567890",
        //            Surname = "TestSurname"
        //        };
        //        context.User.Add(user1);
        //        context.SaveChanges();
        //    }

        //    var mockContext = new Mock<EventPlannerContext>(options);
        //    var controller = new UserController(mockContext.Object);

        //    // Act
        //    var user = new User
        //    {
        //        UserId = 2,
        //        Name = "Another Test User",
        //        Email = "test2@example.com", // Provide values for required properties
        //        Gender = (Gender)2,
        //        Password = "password",
        //        PhoneNumber = "0987654321",
        //        Surname = "AnotherSurname"
        //    };
        //    var result = await controller.AddUser(user);

        //    // Assert
        //    var okResult = Assert.IsType<OkResult>(result);
        //    Assert.Equal(200, okResult.StatusCode);
        //}


        //[Fact]
        //public async Task DeleteUser_ReturnsOkResult_WhenUserExists()
        //{
        //    // Arrange
        //    var options = new DbContextOptionsBuilder<EventPlannerContext>()
        //        .UseInMemoryDatabase(databaseName: "EventPlannerTest")
        //        .Options;
        //    using (var context = new EventPlannerContext(options))
        //    {
        //        context.User.Add(new User { UserId = 1, Name = "Test User" });
        //        context.SaveChanges();
        //    }

        //    var mockContext = new Mock<EventPlannerContext>(options);
        //    var controller = new UserController(mockContext.Object);

        //    // Act
        //    var result = await controller.DeleteUser(1);

        //    // Assert
        //    var okResult = Assert.IsType<OkResult>(result);
        //    Assert.Equal(200, okResult.StatusCode);
        //}

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
            var user = new User { UserId = 1, Name = "Test User" };
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

        //[Fact]
        //public async Task GetUser_NonExistingUser_ReturnsNotFound()
        //{
        //    // Arrange
        //    int nonExistingUserId = 999; // ID of a user that doesn't exist

        //    var mockContext = new Mock<EventPlannerContext>(new DbContextOptions<EventPlannerContext>());
        //    var controller = new UserController(mockContext.Object);

        //    // Act
        //    var result = await controller.GetUser(nonExistingUserId);

        //    // Assert
        //    Assert.IsType<NotFoundResult>(result.Result);
        //}

        //    [Fact]
        //    public async Task UpdateUser_ReturnsBadRequest_WhenModelStateIsInvalid()
        //    {
        //        // Arrange
        //        var options = new DbContextOptionsBuilder<EventPlannerContext>()
        //            .UseInMemoryDatabase(databaseName: "EventPlannerTest")
        //            .Options;
        //        using (var context = new EventPlannerContext(options))
        //        {
        //            context.User.Add(new User { UserId = 1, Name = "Test User" });
        //            context.SaveChanges();
        //        }

        //        var mockContext = new Mock<EventPlannerContext>(options);
        //        var controller = new UserController(mockContext.Object);

        //        // Act
        //        var user = new User { UserId = 1, Name = "" }; // Invalid model state
        //        var result = await controller.UpdateUser(user);

        //        // Assert
        //        Assert.IsType<BadRequestResult>(result);
        //    }
    }
}