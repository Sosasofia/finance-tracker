using System.Net.Http;
using FinanceTracker.Application.Features.Auth.Commands.GoogleLogin;
using FinanceTracker.Application.Features.Auth.Commands.Login;
using FinanceTracker.Application.Features.Auth.Commands.Register;
using FinanceTracker.Application.Features.Auth.Models;
using FinanceTracker.Server.Controllers;
using MediatR;
using Microsoft.AspNetCore.Http;
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

        var httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = await _controller.Login(command);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<AuthResponseDto>(okResult.Value);
        Assert.Equal("valid-token", response.Token);

        _mediatorMock.Verify(
            m => m.Send(command, It.IsAny<CancellationToken>()),
            Times.Once);

        Assert.True(
            httpContext.Response.Headers.TryGetValue("Set-Cookie", out var cookieHeaderValues));

        var cookieHeader = cookieHeaderValues.ToString();

        Assert.Contains("auth_token=valid-token", cookieHeader);
        Assert.Contains("httponly", cookieHeader, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("secure", cookieHeader, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("samesite=none", cookieHeader, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GoogleLogin_WithValidCommand_ReturnsOk()
    {
        // Arrange
        var command = new GoogleLoginCommand("google-token");

        var userSession = new UserSessionDto(Guid.NewGuid(), "test@example.com", "GoogleUser", "User");
        var expectedResponse = new AuthResponseDto("valid-token", userSession);

        _mediatorMock
            .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = await _controller.GoogleLogin(command);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<AuthResponseDto>(okResult.Value);

        Assert.Equal(expectedResponse, response);
        Assert.Equal(expectedResponse.Token, response.Token);

        _mediatorMock.Verify(
            m => m.Send(command, It.IsAny<CancellationToken>()),
            Times.Once);

        Assert.True(
            httpContext.Response.Headers.TryGetValue("Set-Cookie", out var cookieHeaderValues));

        var cookieHeader = cookieHeaderValues.ToString();

        Assert.Contains("auth_token=valid-token", cookieHeader);
        Assert.Contains("httponly", cookieHeader, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("secure", cookieHeader, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("samesite=none", cookieHeader, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Register_WithValidCommand_ReturnsOkAndSetsAuthCookie()
    {
        // Arrange
        var command = new RegisterCommand(
            "newuser@example.com",
            "SecurePassword123",
            "name");

        var userSession = new UserSessionDto(
            Guid.NewGuid(),
            "newuser@example.com",
            "name",
            "User");

        var expectedResponse = new AuthResponseDto(
            "valid-token",
            userSession);

        _mediatorMock
            .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = await _controller.Register(command);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<AuthResponseDto>(okResult.Value);

        Assert.Equal(expectedResponse, response);
        Assert.Equal(expectedResponse.Token, response.Token);

        _mediatorMock.Verify(
            m => m.Send(command, It.IsAny<CancellationToken>()),
            Times.Once);

        Assert.True(
            httpContext.Response.Headers.TryGetValue("Set-Cookie", out var cookieHeaderValues));

        var cookieHeader = cookieHeaderValues.ToString();

        Assert.Contains("auth_token=valid-token", cookieHeader);
        Assert.Contains("httponly", cookieHeader, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("secure", cookieHeader, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("samesite=none", cookieHeader, StringComparison.OrdinalIgnoreCase);
    }


    [Fact]
    public void Logout_ClearsAuthCookie_ReturnsNoContent()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        // Act
        var result = _controller.Logout();

        // Assert
        Assert.IsType<NoContentResult>(result);

        var setCookieHeader = httpContext.Response.Headers["Set-Cookie"].ToString();
        Assert.Contains("auth_token", setCookieHeader);
    }
}
