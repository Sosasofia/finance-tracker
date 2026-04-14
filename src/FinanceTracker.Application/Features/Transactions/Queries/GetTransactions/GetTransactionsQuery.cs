using FinanceTracker.Application.Features.Transactions.Models;
using MediatR;

namespace FinanceTracker.Application.Features.Transactions.Queries.GetTransactionsList;

public record GetTransactionsQuery(Guid UserId) : IRequest<IEnumerable<TransactionResponse>>;
