using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Application.Features.Auth;
using FinanceTracker.Application.Features.Users;
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
            var request = new AuthRequestDto { Email = "test@example.com", Password = "12345678" };
            var fakeResponse = new AuthResponseDto { Token = "fake-jwt", User = new UserResponse { } };

            _authApplicationServiceMock
                .Setup(s => s.LoginUserAsync(request.Email, request.Password))
                .ReturnsAsync(fakeResponse);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<AuthResponseDto>(okResult.Value);
            Assert.Equal("fake-jwt", response.Token);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsBadRequest()
        {
            // Arrange
            var request = new AuthRequestDto { Email = "wrong@example.com", Password = "wrongpass" };

            _authApplicationServiceMock
                .Setup(s => s.LoginUserAsync(request.Email, request.Password))
                .ReturnsAsync((AuthResponseDto?)null);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var message = Assert.IsType<string>(badResult.Value);
            Assert.Equal("Error during login. Try again later.", message);
        }
    }
}
