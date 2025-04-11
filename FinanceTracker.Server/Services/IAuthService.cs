using FinanceTracker.Server.Services.Models;

namespace FinanceTracker.Server.Services
{
    public interface IAuthService
    {
        Task<AuthResponse?> Register(string username, string password);
        Task<AuthResponse?> Login(string username, string password);
    }
}
