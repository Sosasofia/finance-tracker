namespace FinanceTracker.Application.Features.Auth.Models;

public record AuthResponseDto(
    string Token,
    UserSessionDto User
);
