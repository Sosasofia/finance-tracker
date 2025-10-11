using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Application.Features.Auth;
using FinanceTracker.Application.Interfaces;
using FinanceTracker.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthInfrastructureService _authInfrastructureService;
    private readonly IAuthApplicationService _authApplicationService;
    private readonly IUserService _userService;

    public AuthController(
        IAuthApplicationService authApplicationService, 
        IAuthInfrastructureService authInfrastructureService,
        IUserService userService)
    {
        _authApplicationService = authApplicationService;
        _authInfrastructureService = authInfrastructureService;
        _userService = userService;
    }

    [HttpPost("google-login")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
    {
        var payload = await _authInfrastructureService.ValidateGoogleToken(request.IdToken);

        var user = await _userService.FindOrCreateUserAsync(payload.Email, payload.Name, payload.Picture);

        if (user == null)
        {
            return BadRequest("User not found or could not be created.");
        }

        var token = _authInfrastructureService.GenerateToken(user);  

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
