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

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(AuthRequest authRequest)
        {
            var response = await _authService.Login(authRequest.Username, authRequest.Password);

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
            var response = await _authService.Register(authRequest.Username, authRequest.Password);

            if (response == null)
            {
                return BadRequest("User already exists");
            }

            return Ok(response);
        }
    }
}
