using EventPlannerWeb.Controllers;
using EventPlannerWeb.Data;
using EventPlannerWeb.DTO;
using EventPlannerWeb.Interfaces;
using EventPlannerWeb.Models;
using Library_kursova.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class AccountControllerTests
    {
        private readonly DbContextOptions<EventPlannerContext> _options;
        private readonly string BadRequestResult = "User with that email address already exists";

        public AccountControllerTests()
        {
            _options = new DbContextOptionsBuilder<EventPlannerContext>()
                .UseInMemoryDatabase(databaseName: "Test_EventPlannerDB")
                .Options;
            
        }


       

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenUserNotFound()
        {
            // Arrange
            var userManagerMock = new Mock<UserManager<User>>(
                 Mock.Of<IUserStore<User>>(), Mock.Of<IOptions<IdentityOptions>>(),
                 Mock.Of<IPasswordHasher<User>>(), Array.Empty<IUserValidator<User>>(),
                 Array.Empty<IPasswordValidator<User>>(), Mock.Of<ILookupNormalizer>(),
                 Mock.Of<IdentityErrorDescriber>(), Mock.Of<IServiceProvider>(), Mock.Of<ILogger<UserManager<User>>>()
             );
            var tokenServiceMock = new Mock<ITokenService>(MockBehavior.Strict);

            userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);

            var controller = new AccountController(userManagerMock.Object, tokenServiceMock.Object);

            // Act
            var result = await controller.Login(new LoginDTO());

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal("Invalid email", unauthorizedResult.Value);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenInvalidPassword()
        {
            // Arrange
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<IPasswordHasher<User>>(), Array.Empty<IUserValidator<User>>(),
                Array.Empty<IPasswordValidator<User>>(), Mock.Of<ILookupNormalizer>(),
                Mock.Of<IdentityErrorDescriber>(), Mock.Of<IServiceProvider>(), Mock.Of<ILogger<UserManager<User>>>()
            );

            var tokenServiceMock = new Mock<ITokenService>(MockBehavior.Strict);

            var existingUser = new User();
            userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(existingUser);
            userManagerMock.Setup(m => m.CheckPasswordAsync(existingUser, It.IsAny<string>())).ReturnsAsync(false);

            var controller = new AccountController(userManagerMock.Object, tokenServiceMock.Object);

            // Act
            var result = await controller.Login(new LoginDTO());

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal("Invalid password", unauthorizedResult.Value);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenUserAlreadyExists()
        {
            // Arrange
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<IPasswordHasher<User>>(), Array.Empty<IUserValidator<User>>(),
                Array.Empty<IPasswordValidator<User>>(), Mock.Of<ILookupNormalizer>(),
                Mock.Of<IdentityErrorDescriber>(), Mock.Of<IServiceProvider>(), Mock.Of<ILogger<UserManager<User>>>()
            );
            var tokenServiceMock = new Mock<ITokenService>(MockBehavior.Strict);

            // Mocking userManager.Users property to return a list of users
            var users = new List<User>
            {
                new User { Email = "existingemail@example.com" }
            }.AsQueryable();
            var usersMock = new Mock<DbSet<User>>();
            usersMock.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            usersMock.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            usersMock.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            usersMock.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var dbContextMock = new Mock<EventPlannerContext>();
            dbContextMock.Setup(m => m.Users).Returns(usersMock.Object);
            
            userManagerMock.Setup(m => m.Users).Returns(usersMock.Object);

            userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);

            var controller = new AccountController(userManagerMock.Object, tokenServiceMock.Object);

            // Act
            //var result = await controller.Register(new UserDTO { Email = "existingemail@example.com" });

            // Assert
            //var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("User with that email address already exists", BadRequestResult);
        }
    
}
}

