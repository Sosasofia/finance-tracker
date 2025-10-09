namespace FinanceTracker.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string? Name { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? Password { get; set; }

    public string Provider { get; set; } = "local";
    public string? Role { get; set; } = "User";

    public DateTime CreatedAt { get; set; }
    public DateTime LastLoginAt { get; set; }
    public IEnumerable<CustomCategory> CustomCategories { get; set; } = new List<CustomCategory>();
}
