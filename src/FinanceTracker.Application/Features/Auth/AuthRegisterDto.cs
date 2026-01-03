namespace FinanceTracker.Application.Features.Auth;

public class AuthRegisterDto : AuthRequestDto
{
    public string Role { get; set; } = "User";
}
