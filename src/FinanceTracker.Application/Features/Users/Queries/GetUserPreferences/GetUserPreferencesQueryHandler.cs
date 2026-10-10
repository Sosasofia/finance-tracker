using FinanceTracker.Application.Common.Exceptions;
using FinanceTracker.Application.Common.Interfaces.Security;
using FinanceTracker.Application.Features.Users.Models;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Features.Users.Queries.GetUserPreferences;

public class GetUserPreferencesQueryHandler : IRequestHandler<GetUserPreferencesQuery, UserPreferencesResponse>
{
    private readonly IUserPreferencesRepository _userPreferencesRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetUserPreferencesQueryHandler(
        IUserPreferencesRepository userPreferencesRepository,
        ICurrentUserService currentUserService)
    {
        _userPreferencesRepository = userPreferencesRepository;
        _currentUserService = currentUserService;
    }

    public async Task<UserPreferencesResponse> Handle(GetUserPreferencesQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId();

        if (userId == Guid.Empty)
            throw new UnauthorizedAccessException("User is not authenticated.");

        var userPreferences = await _userPreferencesRepository.GetByUserIdAsync(userId);

        if (userPreferences == null)
        {
            userPreferences = UserPreferences.CreateDefault(userId);
            await _userPreferencesRepository.AddAsync(userPreferences);
        }

        return UserPreferencesResponse.MapFrom(userPreferences);
    }
}
