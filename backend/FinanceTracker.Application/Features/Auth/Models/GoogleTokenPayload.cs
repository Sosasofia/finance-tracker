namespace FinanceTracker.Application.Features.Auth.Models;

public record GoogleTokenPayload(
    string Email,
    string Name,
    string Picture
);
