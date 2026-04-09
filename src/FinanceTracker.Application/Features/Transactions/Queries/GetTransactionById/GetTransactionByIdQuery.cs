namespace FinanceTracker.Application.Features.Transactions.Queries.GetTransactionById;

public record GetTransactionByIdQuery(Guid TransactionId, Guid UserId);
