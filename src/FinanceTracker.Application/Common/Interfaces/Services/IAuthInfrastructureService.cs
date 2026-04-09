using FinanceTracker.Application.Features.Auth.Models;

namespace FinanceTracker.Application.Common.Interfaces.Services;

public interface IAuthInfrastructureService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
    Task<GoogleTokenPayload> ValidateGoogleToken(string token);
    string GenerateToken(Guid userID, string email, string name, string role, string provider);
}
