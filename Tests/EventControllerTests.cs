using EventPlannerWeb.Controllers;
using EventPlannerWeb.Data;
using EventPlannerWeb.DTO;
using EventPlannerWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class EventControllerTests
    {
        [Fact]
        public async Task EventPage_ReturnsEventDTO()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<EventPlannerContext>()
                .UseInMemoryDatabase(databaseName: "EventPlannerTest")
                .Options;
            using (var context = new EventPlannerContext(options))
            {
                // Clear existing data
                context.Event.RemoveRange(context.Event);
                await context.SaveChangesAsync();

                // Setup test data
                var testEvent = new Event { EventId = 1, Name = "Test Event" };
                context.Event.Add(testEvent);
                await context.SaveChangesAsync();
            }

        }

        [Fact]
        public async Task AddEvent_InvalidModel_ReturnsBadRequestWithValidationErrors()
        {
            // Arrange
            var invalidEventDTO = new EventDTO { Event = null }; // EventDTO is null

            var mockContext = new Mock<EventPlannerContext>(new DbContextOptions<EventPlannerContext>());
            var controller = new EventController(mockContext.Object); // Instantiate EventController
            controller.ModelState.AddModelError("Event", "The Event field is required."); // Simulate ModelState error

            // Act
            var result = await controller.AddEvent(invalidEventDTO);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorMessage = Assert.IsType<string>(actionResult.Value);
            Assert.Equal("Event data is null", errorMessage);
        }



        //[Fact]
        //public async Task DeleteEvent_ReturnsOkResult_WhenEventExists()
        //{
        //    // Arrange
        //    var options = new DbContextOptionsBuilder<EventPlannerContext>()
        //        .UseInMemoryDatabase(databaseName: "EventPlannerTest")
        //        .Options;

        //    // Initialize the DbContext with test data
        //    using (var context = new EventPlannerContext(options))
        //    {
        //        context.Event.Add(new Event { EventId = 1, Name = "Test Event" });
        //        await context.SaveChangesAsync();
        //    }

        //    // Mock the Event DbSet
        //    var mockSet = new Mock<DbSet<Event>>();
        //    mockSet.Setup(m => m.FindAsync(It.IsAny<int>())).ReturnsAsync((int id) =>
        //    {
        //        using (var innerContext = new EventPlannerContext(options))
        //        {
        //            return innerContext.Event.FirstOrDefaultAsync(e => e.EventId == id);
        //        }
        //    });
        //    mockSet.Setup(m => m.Remove(It.IsAny<Event>())).Callback<Event>((entity) =>
        //    {
        //        using (var innerContext = new EventPlannerContext(options))
        //        {
        //            innerContext.Event.Remove(entity);
        //            innerContext.SaveChanges();
        //        }
        //    });

        //    // Mock the EventPlannerContext to return the mock DbSet
        //    var mockContext = new Mock<EventPlannerContext>(options);
        //    mockContext.Setup(c => c.Event).Returns(mockSet.Object);

        //    // Create the controller with the mocked context
        //    var controller = new EventController(mockContext.Object);

        //    // Act
        //    var result = await controller.DeleteEvent(1);

        //    // Assert
        //    var okResult = Assert.IsType<OkResult>(result);
        //    Assert.Equal(200, okResult.StatusCode);
        //}





        [Fact]
        public async Task UpdateEvent_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var invalidEventDTO = new EventDTO { Event = new Event { Name = null, CreatedDate = DateTime.UtcNow } }; // Name is required

            var mockContext = new Mock<EventPlannerContext>(new DbContextOptions<EventPlannerContext>());
            var controller = new EventController(mockContext.Object);
            controller.ModelState.AddModelError("Name", "The Name field is required."); // Simulate ModelState error

            // Act
            var result = await controller.UpdateEvent(invalidEventDTO);

            // Assert
            Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        }




    }
}
