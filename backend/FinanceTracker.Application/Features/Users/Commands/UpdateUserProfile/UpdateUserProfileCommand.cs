using FinanceTracker.Application.Features.Users.Models;
using MediatR;

namespace FinanceTracker.Application.Features.Users.Commands.UpdateUserProfile;

public record UpdateUserProfileCommand(
    string Name,
    string? ProfilePictureUrl
) : IRequest<UserResponse>;
