using FinanceTracker.Application.Features.Auth;
using FinanceTracker.Application.Features.Users;
using FinanceTracker.Application.Interfaces.Services;
using FinanceTracker.Server.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FinanceTracker.Test.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthApplicationService> _authApplicationServiceMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authApplicationServiceMock = new Mock<IAuthApplicationService>();
            _controller = new AuthController(_authApplicationServiceMock.Object);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOk()
        {
            // Arrange
            var userTest = new UserDto { Email = "test@example.com", Id = new Guid() };
            var request = new AuthRequest { Email = "test@example.com", Password = "12345678" };
            var fakeResponse = new AuthResponse { Token = "fake-jwt", User = userTest};

            _authApplicationServiceMock
                .Setup(s => s.LoginUserAsync(request.Email, request.Password))
                .ReturnsAsync(fakeResponse);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AuthResponse>(okResult.Value);
            Assert.Equal("fake-jwt", response.Token);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsBadRequest()
        {
            // Arrange
            var request = new AuthRequest { Email = "wrong@example.com", Password = "wrongpass" };
            var expectedException = new InvalidOperationException("Invalid credentials");

            _authApplicationServiceMock
                .Setup(s => s.LoginUserAsync(request.Email, request.Password))
                .ThrowsAsync(new Exception("Invalid credentials"));

            // Act & Assert
            var exceptionThrown = await Assert.ThrowsAsync<Exception>(
                async () => await _controller.Login(request)
            );

            Assert.Equal(expectedException.Message, exceptionThrown.Message);
        }
    }
}
