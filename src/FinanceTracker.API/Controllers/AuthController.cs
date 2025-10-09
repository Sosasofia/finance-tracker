using FinanceTracker.Application.Features.Auth;
using FinanceTracker.Application.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthApplicationService _authApplicationService;
    private readonly IUserApplicationService _userAppService;
    private readonly IAuthInfrastructureService _authInfraService;
    public AuthController(
        IAuthApplicationService authService, 
        IUserApplicationService userApplicationService, 
        IAuthInfrastructureService authInfrastructureService)
    {
        _authApplicationService = authService;
        _userAppService = userApplicationService;
        _authInfraService = authInfrastructureService;
    }

    [HttpPost("google-login")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
    {
        var payload = await _authInfraService.ValidateGoogleToken(request.IdToken);

        var user = await _userAppService.FindOrCreateUserAsync(payload.Email, payload.Name, payload.Picture);

        if (user == null)
        {
            return BadRequest("User not found or could not be created.");
        }

        var token = _authInfraService.GenerateToken(user);

        return Ok(new { token });
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] AuthRequest authRequest)
    {
        try
        {
            var response = await _authApplicationService.LoginUserAsync(authRequest.Email, authRequest.Password);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] AuthRequest authRequest)
    {
        try
        {
            var response = await _authApplicationService.RegisterUserAsync(authRequest.Email, authRequest.Password);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}