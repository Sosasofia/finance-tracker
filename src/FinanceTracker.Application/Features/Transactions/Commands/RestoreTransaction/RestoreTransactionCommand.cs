namespace FinanceTracker.Application.Features.Transactions.Commands.RestoreTransaction;

public record RestoreTransactionCommand(Guid TransactionId, Guid UserId);
