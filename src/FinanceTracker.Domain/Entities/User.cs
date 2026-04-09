using FinanceTracker.Domain.Exceptions;

namespace FinanceTracker.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string? Name { get; private set; }
    public string? ProfilePictureUrl { get; private set; }
    public string? Password { get; private set; }

    public string Provider { get; private set; } = "local";
    public string? Role { get; private set; } = "User";

    public DateTime CreatedAt { get; private set; }
    public DateTime LastLoginAt { get; private set; }

    private User() { }

    public static User Create(
        string email, 
        string name, 
        string password, 
        string provider)
    {
        return new User { 
            Email = email, 
            Name = name, 
            Password = password, 
            Provider = provider,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateProfile(string newName, string? newProfilePicture)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new DomainException("The new name is invalid.");

        Name = newName;
        ProfilePictureUrl = newProfilePicture;
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }
}
