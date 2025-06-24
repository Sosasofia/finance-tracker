using FinanceTracker.Server.Models.Entities;
using FinanceTracker.Server.Services.Models;
using Google.Apis.Auth;

namespace FinanceTracker.Server.Services
{
    public interface IAuthService
    {
        Task<AuthResponse?> Register(string username, string password);
        Task<AuthResponse?> Login(string username, string password);
        Task<GoogleJsonWebSignature.Payload> ValidateGoogleToken(string idToken);
        string GenerateToken(User user);
    }
}
