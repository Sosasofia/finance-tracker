using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Application.Features.Transactions.Models;

namespace FinanceTracker.Application.Features.Transactions.Queries.GetTransactionById;

public record GetTransactionByIdQuery(Guid TransactionId, Guid UserId) : IRequest<TransactionResponse>;
