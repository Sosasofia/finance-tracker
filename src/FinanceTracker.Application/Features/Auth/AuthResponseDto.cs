using FinanceTracker.Application.Features.Users;

namespace FinanceTracker.Application.Features.Auth;

public class AuthResponseDto
{
    public string Token { get; set; }
    public UserResponse User { get; set; }
}
