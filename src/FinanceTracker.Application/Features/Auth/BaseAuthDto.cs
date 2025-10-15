namespace FinanceTracker.Application.Features.Auth;

public class BaseAuthDto
{
    public required string Email { get; set; }
    public string Provider { get; set; } = "local";
    public string Name { get; set; }
}
