using FinanceTracker.Application.Common.Interfaces;

namespace FinanceTracker.Application.Features.Transactions.Commands.DeleteTransaction;

public record DeleteTransactionCommand(Guid Id, Guid UserId) : IRequest<Unit>;
