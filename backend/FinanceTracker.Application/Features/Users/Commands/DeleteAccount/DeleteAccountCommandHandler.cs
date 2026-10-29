using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Application.Common.Interfaces.Security;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Features.Users.Commands.DeleteAccount;

public class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public DeleteAccountCommandHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId();
        var user = await _userRepository.FindByIdAsync(userId);

        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found or unauthorized to delete this account.");
        }

        await _userRepository.DeleteAsync(user);

        return Unit.Value;
    }
}
