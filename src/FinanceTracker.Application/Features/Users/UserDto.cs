namespace FinanceTracker.Application.Features.Users;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Role { get; set; }
    public string Provider { get; set; }
}
