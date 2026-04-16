namespace FinanceTracker.Application.Features.Auth.Models;

public record UserSessionDto(
    Guid Id,
    string Name,
    string Email,
    string? Picture
);
