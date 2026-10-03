using FinanceTracker.Application.Common.Interfaces.Security;
using FinanceTracker.Application.Features.Auth.Models;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Features.Users.Queries.GetCurrentUser;
public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserSessionDto>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;

    public GetCurrentUserQueryHandler(
        ICurrentUserService currentUserService,
        IUserRepository userRepository)
    {
        _currentUserService = currentUserService;
        _userRepository = userRepository;
    }

    public async Task<UserSessionDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId();

        if (userId == Guid.Empty)
            throw new UnauthorizedAccessException("User is not authenticated.");

        var user = await _userRepository.FindByIdAsync(userId);

        if (user == null)
            throw new UnauthorizedAccessException("User not found.");

        return new UserSessionDto(
            userId,
            user.Name ?? string.Empty,
            user.Email,
            user.ProfilePictureUrl
        );
    }
}
