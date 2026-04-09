using FinanceTracker.Application.Features.Categories.Models;
using FinanceTracker.Application.Features.Installments.Models;
using FinanceTracker.Application.Features.PaymentMethods.Models;
using FinanceTracker.Application.Features.Reimbursements.Queries;
using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Application.Features.Transactions.Models;

public class TransactionResponse
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "ARS"; 
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public string? Notes { get; set; }
    public string? ReceiptUrl { get; set; }
    public TransactionType Type { get; set; }

    public bool IsCreditCardPurchase { get; set; } 
    public Guid UserId { get; set; }

    public Guid CategoryId { get; set; }
    public CategoryDto? Category { get; set; }

    public Guid PaymentMethodId { get; set; }
    public PaymentMethodDto? PaymentMethod { get; set; }

    public IEnumerable<InstallmentResponse> Installments { get; set; } = [];
    public ReimbursementDto? Reimbursement { get; set; }
}
