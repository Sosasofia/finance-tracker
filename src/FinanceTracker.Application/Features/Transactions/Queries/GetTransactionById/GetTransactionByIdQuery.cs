using FinanceTracker.Application.Features.Transactions.Models;
using MediatR;

namespace FinanceTracker.Application.Features.Transactions.Queries.GetTransactionById;

public record GetTransactionByIdQuery(Guid TransactionId, Guid UserId) : IRequest<TransactionResponse>;
