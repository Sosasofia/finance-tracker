namespace FinanceTracker.Server.Services.Models
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public UserDto User { get; set; }
    }
}

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; }
}