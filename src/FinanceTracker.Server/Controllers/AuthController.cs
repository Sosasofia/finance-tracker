using FinanceTracker.Application.Features.Auth.Commands.GoogleLogin;
using FinanceTracker.Application.Features.Auth.Commands.Login;
using FinanceTracker.Application.Features.Auth.Commands.Register;
using FinanceTracker.Application.Features.Auth.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FinanceTracker.Server.Controllers;

[ApiController]
[Route("api/auth")]
[EnableRateLimiting("auth-limit")]
[Produces("application/json")]
[Consumes("application/json")]
public class AuthController : ControllerBase
{
    private readonly ISender _mediator;

    public AuthController(
            ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Authenticates a user using a Google account and returns an authentication response.
    /// </summary>
    /// <param name="command">The command containing the Google login credentials and related information required for authentication. Cannot be
    /// null.</param>
    /// <returns>An ActionResult containing the authentication response data if the login is successful; otherwise, an error response
    /// indicating the reason for failure.</returns>
    [HttpPost("google-login")]
    public async Task<ActionResult<AuthResponseDto>> GoogleLogin([FromBody] GoogleLoginCommand command)
    {
        var authResponse = await _mediator.Send(command, default);
        return Ok(authResponse);
    }

    /// <summary>
    /// Authenticates a user based on the provided login credentials.
    /// </summary>
    /// <param name="command">The login command containing the user's credentials to be validated. Cannot be null.</param>
    /// <returns>An action result containing the authentication response. Returns a successful result with authentication details
    /// if the credentials are valid; otherwise, returns an error response.</returns>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginCommand command)
    {
        var response = await _mediator.Send(command, default);
        return Ok(response);
    }

    /// <summary>
    /// Registers a new user account using the specified registration details.
    /// </summary>
    /// <param name="command">The registration information required to create a new user account.</param>
    /// <returns>An asynchronous operation that returns an ActionResult containing the authentication response for the newly
    /// registered user.</returns>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterCommand command)
    {
        var response = await _mediator.Send(command, default);
        return Ok(response);
    }
}
