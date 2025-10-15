namespace FinanceTracker.Application.Features.Auth;

public class PasswordAuthDto : BaseAuthDto
{
    public required string Password { get; set; }
}
