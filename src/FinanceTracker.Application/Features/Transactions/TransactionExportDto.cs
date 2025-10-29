using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Application.Features.Transactions;

public class TransactionExportDto
{
    public string Category { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; }
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public TransactionType Type { get; set; }
    public bool HasReimbursement { get; set; }
    public bool IsCreditCardPurchase { get; set; }
}
