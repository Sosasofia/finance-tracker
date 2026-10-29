using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Application.Features.Transactions.Models;

namespace FinanceTracker.Application.Features.Transactions.Commands.UpdateTransaction;

public record UpdateTransactionCommand(
    Guid Id,
    string Name,
    decimal Amount,
    DateTime Date,
    Guid? CategoryId,
    Guid? PaymentMethodId,
    string? Description = null
) : IRequest<TransactionResponse>
{
    public Guid UserId { get; init; }
}
