using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Features.Users.Models;

public class UserResponse
{
    public Guid Id { get; init; }
    public string Email { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string? Picture { get; init; }
    public string Role { get; init; } = null!;
    public string Provider { get; init; } = null!;

    public static UserResponse MapFrom(User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        Name = user.Name ?? string.Empty,
        Picture = user.ProfilePictureUrl,
        Role = user.Role ?? "User",
        Provider = user.Provider
    };
}
