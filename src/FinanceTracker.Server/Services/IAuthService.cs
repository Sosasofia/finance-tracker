using FinanceTracker.Server.Models.DTOs.Response;
using FinanceTracker.Server.Models.Entities;
using Google.Apis.Auth;

namespace FinanceTracker.Server.Services
{
    public interface IAuthService
    {
        Task<AuthResponse?> RegisterUserAsync(string email, string password);
        Task<AuthResponse?> LoginUserAsync(string username, string password);
        Task<GoogleJsonWebSignature.Payload> ValidateGoogleToken(string idToken);
        string GenerateToken(User user);
    }
}
