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
        private readonly DbContextOptions<EventPlannerContext> _options;
        private readonly DbContextOptions<EventPlannerContext> _optionsnew;

        public EventControllerTests()
        {
            _options = new DbContextOptionsBuilder<EventPlannerContext>()
                .UseInMemoryDatabase(databaseName: "Test_EventPlannerDB")
                .Options;
            _optionsnew = new DbContextOptionsBuilder<EventPlannerContext>()
                .UseInMemoryDatabase(databaseName: "Test_EventPlanner")
                .Options;
        }

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

        [Fact]
        public async Task DeleteEvent_ReturnsNotFound_WhenEventNotFound()
        {
            // Arrange
            using (var context = new EventPlannerContext(_options))
            {
                // Initialize database with test data
                context.Event.Add(new Event { EventId = 1, Name = "Test Event" });
                await context.SaveChangesAsync();
            }

            using (var context = new EventPlannerContext(_options))
            {
                var controller = new EventController(context);

                // Act
                var result = await controller.DeleteEvent(2); // Assuming event with ID 2 doesn't exist
                var del = await controller.DeleteEvent(1);

                // Assert
                Assert.IsType<NotFoundResult>(result);
            }
        }
        [Fact]
        public async Task DeleteEvent_ReturnsOkResult_WhenEventDeleted()
        {
            // Arrange
            using (var context = new EventPlannerContext(_options))
            {
                // Initialize database with test data
                var eventData = new Event { EventId = 5, Name = "Test5 Event" };
                context.Event.Add(eventData);
                await context.SaveChangesAsync();
            }

            using (var context = new EventPlannerContext(_options))
            {
                var controller = new EventController(context);

                // Act
                var result = await controller.DeleteEvent(5);

                // Assert
                var okResult = Assert.IsType<OkResult>(result);
                Assert.Equal(200, okResult.StatusCode);
            }
        }

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

        [Fact]
        public async Task EventList_ReturnsViewWithCorrectModel()
        {
            // Arrange
            var expectedEvents = new List<Event>
            {
                new Event { EventId = 11, Name = "Event 11" },
                new Event { EventId = 12, Name = "Event 12" },
                new Event { EventId = 13, Name = "Event 13" }
            };

            using (var context = new EventPlannerContext(_options))
            {
                // Initialize database with test data
                context.Event.AddRange(expectedEvents);
                await context.SaveChangesAsync();
            }

            using (var context = new EventPlannerContext(_options))
            {
                var controller = new EventController(context);

                // Act
                var result = await controller.EventList();

                // Assert
                var actionResult = Assert.IsType<ActionResult<IEnumerable<Event>>>(result);
                var viewResult = Assert.IsAssignableFrom<ViewResult>(actionResult.Result);
                var model = Assert.IsAssignableFrom<IEnumerable<Event>>(viewResult.ViewData.Model);

                // Assert that the model contains the expected events
                Assert.Equal(expectedEvents.Select(e => e.EventId), model.Select(e => e.EventId));
            }
        }
        [Fact]
        public async Task EventPage_ReturnsNotFound_WhenEventNotFound()
        {
            // Arrange
            var nonExistingEventId = 999; // ID of a non-existing event

            using (var context = new EventPlannerContext(_optionsnew))
            {
                // Act
                var controller = new EventController(context);
                var result = await controller.EventPage(nonExistingEventId);

                // Assert
                var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
                Assert.Equal(404, notFoundResult.StatusCode);
            }
        }


        [Fact]
        public async Task AddEventPage_ReturnsViewWithEmptyRecipesAndGuests_WhenNoRecipesOrGuestsExist()
        {
            // Arrange
            using (var context = new EventPlannerContext(_optionsnew))
            {
                // Ensure there are no recipes or guests in the database
                var controller = new EventController(context);

                // Act
                var result = await controller.AddEventPage();

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                var recipes = viewResult.ViewData["Recipes"] as IEnumerable<SelectListItem>;
                var guests = viewResult.ViewData["Guests"] as IEnumerable<SelectListItem>;
                Assert.Empty(recipes); // Assert that recipes ViewData is empty
                Assert.Empty(guests); // Assert that guests ViewData is empty
            }
        }










    }
}
