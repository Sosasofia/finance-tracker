using FinanceTracker.Application.Features.Users.Models;

namespace FinanceTracker.Application.Features.Auth.Models;

public class AuthResponseDto
{
    public string Token { get; set; }
    public UserResponse User { get; set; }
}
