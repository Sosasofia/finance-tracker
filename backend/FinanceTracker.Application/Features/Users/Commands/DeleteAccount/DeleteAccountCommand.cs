using FinanceTracker.Application.Common.Interfaces;

namespace FinanceTracker.Application.Features.Users.Commands.DeleteAccount;

public record DeleteAccountCommand() : IRequest<Unit>;
