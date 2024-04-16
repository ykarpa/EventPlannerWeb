using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using EventPlannerWeb.Controllers;
using EventPlannerWeb.Data;
using EventPlannerWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Xunit;

namespace Tests
{
    public class IngredientControllerTests
    {
        [Fact]
        public async Task IngredientList_ReturnsListOfIngredients()
        {
            // Arrange
            var ingredients = new List<Ingredient>
            {
                new Ingredient { IngredientId = 1, Name = "Ingredient 1", Price = 10 },
                new Ingredient { IngredientId = 2, Name = "Ingredient 2", Price = 20 }
            };

            var mockDbSet = new Mock<DbSet<Ingredient>>();
            mockDbSet.As<IAsyncEnumerable<Ingredient>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<Ingredient>(ingredients.GetEnumerator()));

            mockDbSet.As<IQueryable<Ingredient>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<Ingredient>(ingredients.AsQueryable().Provider));
            mockDbSet.As<IQueryable<Ingredient>>().Setup(m => m.Expression).Returns(ingredients.AsQueryable().Expression);
            mockDbSet.As<IQueryable<Ingredient>>().Setup(m => m.ElementType).Returns(ingredients.AsQueryable().ElementType);
            mockDbSet.As<IQueryable<Ingredient>>().Setup(m => m.GetEnumerator()).Returns(ingredients.AsQueryable().GetEnumerator());

            var options = new DbContextOptionsBuilder<EventPlannerContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            var mockContext = new Mock<EventPlannerContext>(options);
            mockContext.Setup(c => c.Ingredient).Returns(mockDbSet.Object);

            var controller = new IngredientController(mockContext.Object);

            // Act
            var result = await controller.IngredientList();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Ingredient>>>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Ingredient>>(actionResult.Value);
            Assert.Equal(2, model.Count());
        }
        [Fact]
        public async Task AddIngredient_InvalidModel_ReturnsBadRequestWithValidationErrors()
        {
            // Arrange
            var invalidIngredient = new Ingredient { Name = null, Price = 10 }; // Name is required

            var mockContext = new Mock<EventPlannerContext>(new DbContextOptions<EventPlannerContext>());
            var controller = new IngredientController(mockContext.Object);
            controller.ModelState.AddModelError("Name", "The Name field is required."); // Simulate ModelState error

            // Act
            var result = await controller.AddIngredient(invalidIngredient);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorMessage = Assert.IsAssignableFrom<IEnumerable<string>>(actionResult.Value);
            Assert.Contains("The Name field is required.", errorMessage);
        }
    

        [Fact]
        public async Task UpdateIngredient_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var invalidIngredient = new Ingredient { Name = null, Price = 10 }; // Name is required

            var mockContext = new Mock<EventPlannerContext>(new DbContextOptions<EventPlannerContext>());
            var controller = new IngredientController(mockContext.Object);
            controller.ModelState.AddModelError("Name", "The Name field is required."); // Simulate ModelState error

            // Act
            var result = await controller.UpdateIngredient(invalidIngredient);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }




        // Similarly, you can write tests for other actions
    }

    // TestAsyncEnumerator and TestAsyncQueryProvider classes for mocking asynchronous operations
    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_inner.MoveNext());

        public T Current => _inner.Current;

        public ValueTask DisposeAsync() => default;
    }

    internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression) => new TestAsyncEnumerable<TEntity>(expression);

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => new TestAsyncEnumerable<TElement>(expression);

        public object Execute(Expression expression) => _inner.Execute(expression);

        public TResult Execute<TResult>(Expression expression) => _inner.Execute<TResult>(expression);

        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression) => new TestAsyncEnumerable<TResult>(expression);

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken) => Task.FromResult(Execute<TResult>(expression));

        TResult IAsyncQueryProvider.ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        { }

        public TestAsyncEnumerable(Expression expression)
            : base(expression)
        { }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
    }
}
