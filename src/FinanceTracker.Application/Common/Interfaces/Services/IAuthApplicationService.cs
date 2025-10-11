using FinanceTracker.Application.Features.Auth;

namespace FinanceTracker.Application.Interfaces.Services;

public interface IAuthApplicationService
{
    Task<AuthResponse?> RegisterUserAsync(string email, string password);
    Task<AuthResponse?> LoginUserAsync(string username, string password);
}
