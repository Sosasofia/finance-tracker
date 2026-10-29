using FinanceTracker.Application.Common.Interfaces;

namespace FinanceTracker.Application.Features.Users.Commands.ChangePassword;

public record ChangePasswordCommand(
    string CurrentPassword,
    string NewPassword
) : IRequest<Unit>;
