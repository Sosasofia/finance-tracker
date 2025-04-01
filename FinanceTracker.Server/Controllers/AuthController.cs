using FinanceTracker.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Server.Controllers
{
    [ApiController]
    [Route("/api/{controller}")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost]
        [Route("/login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            var token = await _authService.Login(username, password);

            if (token == null)
            {
                return BadRequest("Invalid credentialss");
            }

            return Ok(token);
        }

        [HttpPost]
        [Route("/register")]
        public async Task<IActionResult> Register(string username, string password)
        {
            var user = await _authService.Register(username, password);

            if (user == null)
            {
                return BadRequest("User already exists");
            }

            return Ok("Succesfull registration");
        }
    }
}
