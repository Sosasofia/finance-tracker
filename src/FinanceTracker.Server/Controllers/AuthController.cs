using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Application.Features.Auth;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Server.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
[Consumes("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthApplicationService _authApplicationService;

    public AuthController(IAuthApplicationService authApplicationService)
    {
        _authApplicationService = authApplicationService;
    }

    /// <summary>
    /// Logs in or registers a user using a Google ID token.
    /// </summary>
    /// <param name="request">The Google login request containing the ID token.</param>
    /// <response code="200">Returns the auth response with a JWT and user details.</response>
    /// <response code="400">If the ID token is missing from the request.</response>
    /// <response code="401">If the Google token is invalid or authentication fails.</response>
    [HttpPost("google-login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> GoogleLogin([FromBody] GoogleLoginRequest request)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.IdToken))
        {
            return BadRequest(new { message = "Id token is required." });
        }

        var authResponse = await _authApplicationService.AuthenticateWithGoogleAsync(request.IdToken);

        return Ok(authResponse);
    }

    /// <summary>
    /// Logs in a user with email and password.
    /// </summary>
    /// <param name="authRequest">The user's login credentials.</param>
    /// <response code="200">Returns the auth response with a JWT and user details.</response>
    /// <response code="401">If the email or password is incorrect.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] AuthRequestDto authRequest)
    {
        var response = await _authApplicationService.LoginUserAsync(authRequest.Email, authRequest.Password);

        return response == null ? BadRequest("Error during login. Try again later.") : Ok(response);
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="authRegisterReq">The new user's email and password.</param>
    /// <response code="200">Returns the auth response with a JWT and user details.</response>
    /// <response code="400">If email or password validation fails (e.g., short password).</response>
    /// <response code="409">If an account with that email already exists.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status409Conflict)] // Documenting the 409
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] AuthRegisterDto authRegisterReq)
    {
        var response = await _authApplicationService.RegisterUserAsync(authRegisterReq.Email, authRegisterReq.Password);

        return response == null ? BadRequest("Error during registration. Try again later.") : Ok(response);
    }
}
