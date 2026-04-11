using FinanceTracker.Application.Features.Transactions.Models;
using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Application.Features.Transactions.Commands.CreateTransaction;

public record CreateTransactionCommand
{
    public decimal Amount { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime Date { get; init; }
    public string? Notes { get; init; }
    public string? ReceiptUrl { get; init; }
    public TransactionType Type { get; init; } = TransactionType.Expense;
    public Guid CategoryId { get; init; }
    public Guid PaymentMethodId { get; init; }

    public Guid UserId { get; set; }

    public bool IsCreditCardPurchase { get; init; }
    public InstallmentDto? Installment { get; init; }

    public bool IsReimbursement { get; init; }
    public ReimbursementDto? Reimbursement { get; init; }
}
