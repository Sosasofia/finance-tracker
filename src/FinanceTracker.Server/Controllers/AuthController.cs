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
    /// Authenticate a user using a Google ID token.
    /// </summary>
    /// 
    /// <remarks>
    /// <p><strong>Description:</strong> This endpoint accepts a Google ID token obtained on the client and will either:</p>
    /// <ul>
    ///   <li>Validate the token and sign in the existing user, or</li>
    ///   <li>Create a new user (if allowed) and return an authentication response.</li>
    /// </ul>
    /// 
    /// <p><strong>Details:</strong></p>
    /// <ul>
    ///   <li>The ID token must come from Google's OAuth sign-in flow.</li>
    ///   <li>The token must not be expired or tampered with.</li>
    ///   <li>A valid token returns a JWT used for subsequent authenticated requests.</li>
    /// </ul>
    /// 
    /// <p><strong>Result:</strong>  
    /// <c>200 OK</c> with an <strong>AuthResponseDto</strong> containing the JWT and user information.</p>
    ///
    /// </remarks>
    /// 
    /// <param name="request">Contains the Google ID token.</param>
    /// <response code="200">Returns the auth response with a JWT token and user details.</response>
    /// <response code="400">If the ID token is missing from the request.</response>
    /// <response code="401">If the Google token is invalid or authentication fails.</response>
    [HttpPost("google-login")]
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
    /// Logs in a user using email and password credentials.
    /// </summary>
    /// 
    /// <remarks>
    /// <p><strong>Description:</strong> Validates the user's credentials and returns an authentication token.</p>
    /// 
    /// <p><strong>Details:</strong></p>
    /// <ul>
    ///   <li>The email must exist in the system.</li>
    ///   <li>The password must match the registered credentials.</li>
    ///   <li>Returns a JWT (Bearer token) for authenticated API access.</li>
    /// </ul>
    /// 
    /// <p><strong>Result:</strong>  
    /// <c>200 OK</c> with an <strong>AuthResponseDto</strong> containing user info and JWT.</p>
    ///
    /// </remarks>
    /// <param name="authRequest">The user's login credentials.</param>
    /// <response code="200">Returns the auth response with a JWT token and user details.</response>
    /// <response code="401">If the email or password is incorrect.</response>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] AuthRequestDto authRequest)
    {
        var response = await _authApplicationService.LoginUserAsync(authRequest.Email, authRequest.Password);

        return response == null ? BadRequest("Error during login. Try again later.") : Ok(response);
    }

    /// <summary>
    /// Registers a new user using an email and password.
    /// </summary>
    /// 
    /// <remarks>
    /// <p><strong>Description:</strong>  
    /// Creates a new user account and returns an authentication token.</p>
    /// 
    /// <p><strong>Details:</strong></p>
    /// <ul>
    ///   <li>Email must be unique (non-existing).</li>
    ///   <li>Password must satisfy complexity rules.</li>
    ///   <li>After registration, the user is automatically authenticated.</li>
    /// </ul>
    ///
    /// <p><strong>Result:</strong>  
    /// <c>200 OK</c> with <strong>AuthResponseDto</strong> including the JWT.</p>
    ///
    /// </remarks>
    /// <param name="authRegisterReq">The registration details including email, password, and optional role.</param>
    /// <response code="200">Returns the authentication response with a JWT and user details.</response>
    /// <response code="400">Returned when email or password validation fails.</response>
    /// <response code="409">Returned when an account with the provided email already exists.</response>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] AuthRegisterDto authRegisterReq)
    {
        var response = await _authApplicationService.RegisterUserAsync(authRegisterReq.Email, authRegisterReq.Password);

        return response == null ? BadRequest("Error during registration. Try again later.") : Ok(response);
    }
}
