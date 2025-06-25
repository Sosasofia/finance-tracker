using FinanceTracker.Server.Services;
using FinanceTracker.Server.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            var payload = await _authService.ValidateGoogleToken(request.IdToken);

            var user = await _userService.FindOrCreateUserAsync(payload.Email, payload.Name, payload.Picture);

            if (user == null)
            {
                return BadRequest("User not found or could not be created.");
            }

            var token = _authService.GenerateToken(user);  

            return Ok(new { token }); 
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(AuthRequest authRequest)
        {
            var response = await _authService.LoginUserAsync(authRequest.Email, authRequest.Password);

            if (response == null)
            {
                return BadRequest("Invalid credentials");
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(AuthRequest authRequest)
        {
            var response = await _authService.RegisterUserAsync(authRequest.Email, authRequest.Password);

            if (response == null)
            {
                return BadRequest("User already exists");
            }

            return Ok(response);
        }
    }
}

public class GoogleLoginRequest
{
    public string IdToken { get; set; }
}