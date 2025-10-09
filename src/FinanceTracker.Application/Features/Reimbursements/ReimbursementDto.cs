using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Application.Features.Reimbursements;

public class ReimbursementDto
{
    [Required]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Reimbursement amount must be greater than zero.")]
    public decimal Amount { get; set; }
    [Required]

    public DateTime Date { get; set; }
    public string? Reason { get; set; }

    public List<string> Validate()
    {
        var errors = new List<string>();

        if (Date > DateTime.UtcNow)
        {
            errors.Add("Transaction date cannot be in the future.");
        }

        return errors;
    }
}