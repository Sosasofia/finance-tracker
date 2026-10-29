using FinanceTracker.Application.Common.Exceptions;
using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Application.Common.Interfaces.Security;
using FinanceTracker.Application.Features.Users.Models;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Features.Users.Commands.UpdateUserPreferences;

public class UpdateUserPreferencesCommandHandler : IRequestHandler<UpdateUserPreferencesCommand, UserPreferencesResponse>
{
    private readonly IUserPreferencesRepository _userPreferencesRepository;
    private readonly ICurrentUserService _currentUserService;
    public UpdateUserPreferencesCommandHandler(
        IUserPreferencesRepository userPreferencesRepository,
        ICurrentUserService currentUserService)
    {
        _userPreferencesRepository = userPreferencesRepository;
        _currentUserService = currentUserService;
    }
    public async Task<UserPreferencesResponse> Handle(UpdateUserPreferencesCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId();
        var userPreferences = await _userPreferencesRepository.GetByUserIdAsync(userId);

        if (userPreferences == null)
        {
            userPreferences = UserPreferences.CreateDefault(userId);
            userPreferences.Update(request.DefaultCurrency, request.DateFormat);
            await _userPreferencesRepository.AddAsync(userPreferences);
        }
        else
        {
            userPreferences.Update(request.DefaultCurrency, request.DateFormat);
            await _userPreferencesRepository.UpdateAsync(userPreferences);
        }

        return UserPreferencesResponse.MapFrom(userPreferences);
    }
}
