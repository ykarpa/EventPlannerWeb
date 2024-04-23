using EventPlannerWeb.Controllers;
using EventPlannerWeb.Data;
using EventPlannerWeb.DTO;
using EventPlannerWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class GuestControllerTests
    {
        private readonly DbContextOptions<EventPlannerContext> _options;

        public GuestControllerTests()
        {
            _options = new DbContextOptionsBuilder<EventPlannerContext>()
                .UseInMemoryDatabase(databaseName: "Test_EventPlannerDB")
                .Options;
        }
        [Fact]
        public async Task AddGuestPage_ReturnsViewWithEmptyGuests_WhenNoGuestsExist()
        {
            // Arrange
            using (var context = new EventPlannerContext(_options))
            {
                // Ensure there are no guests in the database
                var controller = new GuestController(context);

                // Act
                var result = await controller.AddGuestPage();

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                var guests = viewResult.ViewData["Guests"] as IEnumerable<SelectListItem>;
                Assert.Empty(guests); // Assert that the list of guests is empty
            }
        }


        [Fact]
        public async Task GuestList_ReturnsListOfGuests()
        {
                    // Arrange
            var guests = new List<Guest>
            {
                new Guest { GuestId = 1, Name = "Guest 1", Gender = Gender.Male },
                new Guest { GuestId = 2, Name = "Guest 2", Gender = Gender.Female }
            };

            var mockDbSet = new Mock<DbSet<Guest>>();
            mockDbSet.As<IAsyncEnumerable<Guest>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<Guest>(guests.GetEnumerator()));

            mockDbSet.As<IQueryable<Guest>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<Guest>(guests.AsQueryable().Provider));
            mockDbSet.As<IQueryable<Guest>>().Setup(m => m.Expression).Returns(guests.AsQueryable().Expression);
            mockDbSet.As<IQueryable<Guest>>().Setup(m => m.ElementType).Returns(guests.AsQueryable().ElementType);
            mockDbSet.As<IQueryable<Guest>>().Setup(m => m.GetEnumerator()).Returns(guests.AsQueryable().GetEnumerator());

            var options = new DbContextOptionsBuilder<EventPlannerContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            var mockContext = new Mock<EventPlannerContext>(options);
            mockContext.Setup(c => c.Guest).Returns(mockDbSet.Object); // Corrected this line

            var controller = new GuestController(mockContext.Object);

            // Act
            var result = await controller.GuestList();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Guest>>>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Guest>>(actionResult.Value);
            Assert.Equal(2, model.Count());
        }



        [Fact]
        public async Task AddGuest_InvalidModel_ReturnsBadRequestWithValidationErrors()
        {
            // Arrange
            var invalidGuest = new Guest { Name = null, Gender = Gender.Male }; // Name is required

            var mockContext = new Mock<EventPlannerContext>(new DbContextOptions<EventPlannerContext>());
            var controller = new GuestController(mockContext.Object);
            controller.ModelState.AddModelError("Name", "The Name field is required."); // Simulate ModelState error

            // Act
            var result = await controller.AddGuest(invalidGuest);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorMessage = Assert.IsAssignableFrom<IEnumerable<string>>(actionResult.Value);
            Assert.Contains("The Name field is required.", errorMessage);
        }

        [Fact]
        public async Task UpdateGuest_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var invalidGuest = new Guest { Name = null, Gender = Gender.Male }; // Name is required

            var mockContext = new Mock<EventPlannerContext>(new DbContextOptions<EventPlannerContext>());
            var controller = new GuestController(mockContext.Object);
            controller.ModelState.AddModelError("Name", "The Name field is required."); // Simulate ModelState error

            // Act
            var result = await controller.UpdateGuest(invalidGuest);

            // Assert
            Assert.IsAssignableFrom<BadRequestResult>(result);
        }
        [Fact]
        public async Task DeleteGuest_ReturnsNotFound_WhenGuestNotFound()
        {
            // Arrange
            using (var context = new EventPlannerContext(_options))
            {
                // Initialize database with test data
                context.Guest.Add(new Guest { GuestId = 1, Name = "Test Guest", Surname="Test" });
                await context.SaveChangesAsync();
            }

            using (var context = new EventPlannerContext(_options))
            {
                var controller = new GuestController(context);

                // Act
                var result = await controller.DeleteGuest(2); // Assuming guest with ID 2 doesn't exist
                var del = await controller.DeleteGuest(1);

                // Assert
                Assert.IsType<NotFoundResult>(result);
            }
        }

        [Fact]
        public async Task DeleteGuest_ReturnsOkResult_WhenGuestDeleted()
        {
            // Arrange
            using (var context = new EventPlannerContext(_options))
            {
                // Initialize database with test data
                var guestData = new Guest { GuestId = 5, Name = "Test Guest 5", Surname = "Test" };
                context.Guest.Add(guestData);
                await context.SaveChangesAsync();
            }

            using (var context = new EventPlannerContext(_options))
            {
                var controller = new GuestController(context);

                // Act
                var result = await controller.DeleteGuest(5);

                // Assert
                var okResult = Assert.IsType<OkResult>(result);
                Assert.Equal(200, okResult.StatusCode);
            }
        }
        [Fact]
        public async Task UpdateGuestPage_ReturnsViewWithGuest()
        {
            // Arrange
            var expectedGuest = new Guest { GuestId = 1, Name = "Test Guest", Surname = "Test" };

            var mockDbSet = new Mock<DbSet<Guest>>();
            mockDbSet.Setup(m => m.FindAsync(1)).ReturnsAsync(expectedGuest);

            var options = new DbContextOptionsBuilder<EventPlannerContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            var mockContext = new Mock<EventPlannerContext>(options);
            mockContext.Setup(c => c.Guest).Returns(mockDbSet.Object);

            var controller = new GuestController(mockContext.Object);

            // Act
            var result = await controller.UpdateGuestPage(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Guest>(viewResult.ViewData.Model);
            Assert.Equal(expectedGuest, model);
        }

        [Fact]
        public async Task UpdateGuestPage_ReturnsNotFound_WhenGuestNotFound()
        {
            // Arrange
            var mockDbSet = new Mock<DbSet<Guest>>();
            mockDbSet.Setup(m => m.FindAsync(2)).ReturnsAsync((Guest)null); // Assuming guest with ID 2 doesn't exist

            var options = new DbContextOptionsBuilder<EventPlannerContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            var mockContext = new Mock<EventPlannerContext>(options);
            mockContext.Setup(c => c.Guest).Returns(mockDbSet.Object);

            var controller = new GuestController(mockContext.Object);

            // Act
            var result = await controller.UpdateGuestPage(2);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public async Task GuestListPage_ReturnsViewWithEmptyList_WhenNoGuestsExist()
        {
            // Arrange
            using (var context = new EventPlannerContext(_options))
            {
                // Ensure there are no guests in the database
                var controller = new GuestController(context);

                // Act
                var result = await controller.GuestListPage();

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsAssignableFrom<IEnumerable<Guest>>(viewResult.Model);
                Assert.Empty(model); // Assert that the model is an empty list
            }
        }
        [Fact]
        public async Task GuestPage_ReturnsNotFound_WhenGuestNotFound()
        {
            // Arrange
            var nonExistingGuestId = 999; // ID of a non-existing guest

            using (var context = new EventPlannerContext(_options))
            {
                // No guest is added to the database
                var controller = new GuestController(context);

                // Act
                var result = await controller.GuestPage(nonExistingGuestId);

                // Assert
                Assert.IsType<NotFoundResult>(result);
            }
        }

        


    }
}
