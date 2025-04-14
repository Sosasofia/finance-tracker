using FinanceTracker.Server.Models;
using FinanceTracker.Server.Models.Entities;
using FinanceTracker.Server.Services.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FinanceTracker.Server.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly Context _context;

        public AuthService(IConfiguration configuration, Context context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task<AuthResponse?> Register(string username, string password)
        {
            if (_context.Users.Any(u => u.Username == username))
                return null;

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User { Password = passwordHash, Username = username };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = GenerateToken(user);

            return new AuthResponse
            {
                Token = token,
                Username = user.Username
            };
        }

        public async Task<AuthResponse?> Login(string username, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                return null;

            var token = GenerateToken(user);

            return new AuthResponse
            {
                Token = token,
                Username = user.Username
            };
        }

        private string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim(ClaimTypes.Name, user.Username),
                }),
                Expires = DateTime.UtcNow.Add(TimeSpan.FromMinutes(90)),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            string userToken = tokenHandler.WriteToken(token);

            return userToken;
        }
    }
}
