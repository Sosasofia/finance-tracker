using System.ComponentModel.DataAnnotations;
using FinanceTracker.Application.Common.Validators;
using FinanceTracker.Application.Features.Installments;
using FinanceTracker.Application.Features.Reimbursements;
using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Application.Features.Transactions;

public class CreateTransactionDto
{
    [Required]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Transaction amount must be greater than zero.")]
    required public decimal Amount { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 3)]
    required public string Name { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    [DataCannotBeInTheFuture]
    required public DateTime Date { get; set; }

    public string? Notes { get; set; }

    public string? ReceiptUrl { get; set; }

    public TransactionType Type { get; set; } = TransactionType.Expense;

    [Required]
    public Guid? CategoryId { get; set; }

    [Required]
    public Guid? PaymentMethodId { get; set; }

    // Credit card
    public bool IsCreditCardPurchase { get; set; } = false;
    public InstallmentDto? Installment { get; set; }

    // Reimbursement
    public bool IsReimbursement { get; set; } = false;
    public ReimbursementDto? Reimbursement { get; set; }
}
