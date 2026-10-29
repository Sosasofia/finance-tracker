using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Application.Features.Transactions.Models;

namespace FinanceTracker.Application.Features.Transactions.Commands.RestoreTransaction;

public record RestoreTransactionCommand(Guid TransactionId, Guid UserId) : IRequest<TransactionResponse>;
