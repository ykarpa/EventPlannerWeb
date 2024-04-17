using EventPlannerWeb.Controllers;
using EventPlannerWeb.Data;
using EventPlannerWeb.DTO;
using EventPlannerWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class GuestControllerTests
    {
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

    }
}
