using FinanceTracker.Application.Features.Transactions.Models;
using FinanceTracker.Domain.Enums;
using MediatR;

namespace FinanceTracker.Application.Features.Transactions.Commands.CreateTransaction;

public record CreateTransactionCommand(
    decimal Amount,
    string Name,
    DateTime Date,
    TransactionType Type,
    Guid CategoryId,
    Guid PaymentMethodId,
    string? Description = null,
    string? Notes = null,
    string? ReceiptUrl = null,
    bool IsCreditCardPurchase = false,
    InstallmentDto? Installment = null,
    bool IsReimbursement = false,
    ReimbursementDto? Reimbursement = null
) : IRequest<TransactionResponse>
{
    public Guid UserId { get; init; }
}
