using FinanceTracker.Application.Common.Interfaces.Security;
using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Domain.Interfaces;
using MediatR;

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
        var isPasswordValid = _authInfrastructureService.VerifyPassword(request.CurrentPassword, user.Password);

        if (user == null || !isPasswordValid)
        {
            throw new UnauthorizedAccessException("Invalid operation.");
        }

        var newPasswordHash = _authInfrastructureService.HashPassword(request.NewPassword);
        user.ChangePassword(newPasswordHash);
        await _userRepository.UpdateAsync(user);

        return Unit.Value;
    }
}

