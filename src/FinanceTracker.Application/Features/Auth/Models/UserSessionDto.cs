namespace FinanceTracker.Application.Features.Auth.Models;

public record UserSessionDto(
    string Name,
    string Email,
    string? Picture
);
