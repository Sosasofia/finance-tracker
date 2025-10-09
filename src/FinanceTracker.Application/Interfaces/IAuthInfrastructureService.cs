using FinanceTracker.Application.Features.Auth;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Interfaces;

public interface IAuthInfrastructureService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
    Task<GoogleTokenPayload> ValidateGoogleToken(string token); 
    string GenerateToken(User user);
}
