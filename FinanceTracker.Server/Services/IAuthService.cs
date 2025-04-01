using FinanceTracker.Server.Services.Models;

namespace FinanceTracker.Server.Services
{
    public interface IAuthService
    {
        Task<User?> Register(string username, string password);
        Task<string?> Login(string username, string password);
    }
}
