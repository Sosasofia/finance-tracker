using FinanceTracker.Application.Features.Auth;
using FinanceTracker.Application.Features.Users;

namespace FinanceTracker.Application.Common.Interfaces.Services;

public interface IAuthInfrastructureService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
    Task<GoogleTokenPayload> ValidateGoogleToken(string token);
    string GenerateToken(UserDto user);
}
