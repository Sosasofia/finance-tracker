using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Application.Common.Interfaces.Security;
using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Domain.Exceptions;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Features.Users.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthInfrastructureService _authInfrastructureService;
    private readonly ICurrentUserService _currentUserService;

    public ChangePasswordCommandHandler(
        IUserRepository userRepository,
        IAuthInfrastructureService authInfrastructureService,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _authInfrastructureService = authInfrastructureService;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId();
        var user = await _userRepository.FindByIdAsync(currentUserId);

        if (user == null || !_authInfrastructureService.VerifyPassword(request.CurrentPassword, user.Password!))
        {
            throw new DomainException("The current password provided is incorrect.");
        }

        var newPasswordHash = _authInfrastructureService.HashPassword(request.NewPassword);
        user.ChangePassword(newPasswordHash);
        await _userRepository.UpdateAsync(user);

        return Unit.Value;
    }
}

