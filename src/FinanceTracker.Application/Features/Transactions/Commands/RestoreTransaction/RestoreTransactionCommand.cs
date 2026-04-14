using FinanceTracker.Application.Features.Transactions.Models;
using MediatR;

namespace FinanceTracker.Application.Features.Transactions.Commands.RestoreTransaction;

public record RestoreTransactionCommand(Guid TransactionId, Guid UserId) : IRequest<TransactionResponse>;
