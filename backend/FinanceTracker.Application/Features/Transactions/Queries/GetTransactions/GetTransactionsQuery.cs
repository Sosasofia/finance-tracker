using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Application.Features.Transactions.Models;

namespace FinanceTracker.Application.Features.Transactions.Queries.GetTransactionsList;

public record GetTransactionsQuery(Guid UserId) : IRequest<IEnumerable<TransactionResponse>>;
