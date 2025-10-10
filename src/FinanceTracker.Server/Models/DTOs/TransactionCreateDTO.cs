using FinanceTracker.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Server.Models.DTOs;

public class TransactionCreateDTO
{
    [Required]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Transaction amount must be greater than zero.")]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    public DateTime Date { get; set; }
    public string? Notes { get; set; }
    public string? ReceiptUrl { get; set; }
    public TransactionType Type { get; set; } = TransactionType.Expense;
    public Guid CategoryId { get; set; }
    public Guid PaymentMethodId { get; set; }

    // Credit card
    public bool IsCreditCardPurchase { get; set; } = false;
    public InstallmentDTO? Installment { get; set; }

    // Reimburstment
    public bool IsReimbursement { get; set; } = false;
    public ReimbursementDTO? Reimbursement { get; set; }

    public List<string> Validate()
    {
        var errors = new List<string>();

        if (IsReimbursement && Reimbursement == null)
        {
            errors.Add("Reimbursement details must be provided if the transaction is marked as a reimbursement.");
        }

        if (IsCreditCardPurchase && Installment == null)
        {
            errors.Add("Installment details must be provided if the transaction is marked as a credit card purchase.");
        }

        if (Date > DateTime.UtcNow)
        {
            errors.Add($"Transaction date cannot be in the future.");
        }

        if (IsReimbursement && Reimbursement != null)
        {
            errors.AddRange(Reimbursement.Validate()); 
        }

        if(Type == TransactionType.Expense && CategoryId == null)
        {
            errors.Add("CategoryId is required for expense transactions.");
        }

        if (Type == TransactionType.Expense && PaymentMethodId == null)
        {
            errors.Add("PaymentMethodId is required for expense transactions.");
        }

        return errors;
    }
}
