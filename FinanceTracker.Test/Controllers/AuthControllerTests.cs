using FinanceTracker.Server.Controllers;
using FinanceTracker.Server.Models.DTOs;
using FinanceTracker.Server.Models.DTOs.Request;
using FinanceTracker.Server.Models.DTOs.Response;
using FinanceTracker.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FinanceTracker.Test.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _userServiceMock = new Mock<IUserService>();
            _controller = new AuthController(_authServiceMock.Object, _userServiceMock.Object);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOk()
        {
            // Arrange
            var userTest = new UserDTO { Email = "test@example.com", Id = new Guid() };
            var request = new AuthRequest { Email = "test@example.com", Password = "12345678" };
            var fakeResponse = new AuthResponse { Token = "fake-jwt", User = userTest};

            _authServiceMock
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

            _authServiceMock
                .Setup(s => s.LoginUserAsync(request.Email, request.Password))
                .ThrowsAsync(new Exception("Invalid credentials"));

            // Act
            var result = await _controller.Login(request);
            //return BadRequest(new { error = ex.Message });


            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var error = Assert.IsType<string>(badRequest.Value);
            Assert.Equal("Invalid credentials", error);
            Assert.Equal(400, badRequest.StatusCode);   
            //Assert.Equal(new { error = ex.Message });
        }
    }
}
