using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Features.Users.Models;

public record UserResponse(
    Guid Id,
    string Email,
    string Name,
    string? Picture,
    string Role,
    string Provider
)
{
    public static UserResponse MapFrom(User user) => new(
        Id: user.Id,
        Email: user.Email,
        Name: user.Name ?? string.Empty,
        Picture: user.ProfilePictureUrl,
        Role: user.Role ?? "User",
        Provider: user.Provider
    );
}
