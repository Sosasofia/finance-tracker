namespace FinanceTracker.Application.Features.Auth.Models;

public class AuthResponseDto
{
    public string Token { get; set; } = null!;
    public UserSessionDto User { get; set; } = null!;
}
