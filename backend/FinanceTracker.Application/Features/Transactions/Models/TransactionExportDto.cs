using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Application.Features.Transactions.Models;

public record TransactionExportDto(
    string Category,
    decimal Amount,
    string PaymentMethod,
    string Name,
    DateTime Date,
    TransactionType Type,
    bool HasReimbursement,
    bool IsCreditCardPurchase,
    string Currency = "ARS",
    string? Description = null,
    string? Notes = null
)
{
    public static TransactionExportDto MapFrom(Transaction src) => new(
        Category: src.Category?.Name ?? "Uncategorized",
        Amount: src.Money.Amount,
        PaymentMethod: src.PaymentMethod?.Name ?? "N/A",
        Name: src.Name,
        Date: src.Date,
        Type: src.Type,
        HasReimbursement: src.Reimbursement != null,
        IsCreditCardPurchase: src.IsCreditCardPurchase,
        Currency: src.Money.Currency,
        Description: src.Description,
        Notes: src.Notes
    );
}
