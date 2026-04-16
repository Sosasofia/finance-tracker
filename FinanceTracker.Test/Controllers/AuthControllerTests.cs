using FinanceTracker.Application.Features.Auth.Commands.GoogleLogin;
using FinanceTracker.Application.Features.Auth.Commands.Login;
using FinanceTracker.Application.Features.Auth.Commands.Register;
using FinanceTracker.Application.Features.Auth.Models;
using FinanceTracker.Server.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FinanceTracker.Test.Controllers;

public class AuthControllerTests
{
    private readonly Mock<ISender> _mediatorMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mediatorMock = new Mock<ISender>();
        _controller = new AuthController(_mediatorMock.Object);
    }

    [Fact]
    public async Task Login_WithValidCommand_ReturnsOk()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "password123");

        var userSession = new UserSessionDto(Guid.NewGuid(), "test@example.com", "User", "User");
        var expectedResponse = new AuthResponseDto("valid-token", userSession);

        _mediatorMock
            .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Login(command);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<AuthResponseDto>(okResult.Value);
        Assert.Equal("valid-token", response.Token);
    }

    [Fact]
    public async Task GoogleLogin_WithValidCommand_ReturnsOk()
    {
        // Arrange
        var command = new GoogleLoginCommand("google-token");

        var userSession = new UserSessionDto(Guid.NewGuid(), "test@example.com", "GoogleUser", "User");
        var expectedResponse = new AuthResponseDto("google-auth-token", userSession);

        _mediatorMock
            .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GoogleLogin(command);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<AuthResponseDto>(okResult.Value);
        Assert.Equal("google-auth-token", response.Token);
    }

    [Fact]
    public async Task Register_WithValidCommand_ReturnsOk()
    {
        // Arrange
        var command = new RegisterCommand("newuser@example.com", "SecurePassword123", "name");

        var userSession = new UserSessionDto(Guid.NewGuid(), "newuser@example.com", "name", "User");
        var expectedResponse = new AuthResponseDto("new-user-token", userSession);

        _mediatorMock
            .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Register(command);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<AuthResponseDto>(okResult.Value);
        Assert.Equal("new-user-token", response.Token);
    }
}
