using FinanceTracker.Application.Common.Exceptions;
using FinanceTracker.Application.Common.Interfaces.Security;
using FinanceTracker.Application.Features.Users.Models;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Features.Users.Commands.UpdateUserProfile;

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, UserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateUserProfileCommandHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository ;
        _currentUserService = currentUserService;
    }

    public async Task<UserResponse> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId();
        var user = await _userRepository.FindByIdAsync(userId);

        if (user == null)
            throw new NotFoundException($"User not found.");

        user.UpdateProfile(request.Name, request.ProfilePictureUrl);

        await _userRepository.UpdateAsync(user);

        return UserResponse.MapFrom(user);
    }
}
