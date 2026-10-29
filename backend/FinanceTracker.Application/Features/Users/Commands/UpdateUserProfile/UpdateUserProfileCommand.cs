using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Application.Features.Users.Models;

namespace FinanceTracker.Application.Features.Users.Commands.UpdateUserProfile;

public record UpdateUserProfileCommand(
    string Name,
    string? ProfilePictureUrl
) : IRequest<UserResponse>;
