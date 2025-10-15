using FinanceTracker.Application.Features.Auth;
using FinanceTracker.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthApplicationService _authApplicationService;

    public AuthController(IAuthApplicationService authApplicationService)
    {
        _authApplicationService = authApplicationService;
    }

    [HttpPost("google-login")]
    public async Task<ActionResult<string>> GoogleLogin([FromBody] GoogleLoginRequest request)
    {

        var token = await _authApplicationService.AuthenticateWithGoogleAsync(request.IdToken);

        if (token == null)
        {
            return Unauthorized(new { message = "Invalid Google token or authentication failed." });
        }

        return Ok(new { token.Token });
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] AuthRequest authRequest)
    {
        var response = await _authApplicationService.LoginUserAsync(authRequest.Email, authRequest.Password);

        if (response == null)
        {
            return BadRequest("Error during login. Try again later.");
        }

        return Ok(response);
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] AuthRequest authRequest)
    {

        var response = await _authApplicationService.RegisterUserAsync(authRequest.Email, authRequest.Password);

        if (response == null)
        {
            return BadRequest("Error during registration. Try again later.");
        }

        return Ok(response);
    }
}
