using System.ComponentModel.DataAnnotations;
using FinanceTracker.Application.Common.Validators;

namespace FinanceTracker.Application.Features.Reimbursements;

public class ReimbursementDto
{
    [Required]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Reimbursement amount must be greater than zero.")]
    public decimal Amount { get; set; }
    [Required]
    [DataCannotBeInTheFuture]
    public DateTime Date { get; set; }
    public string? Reason { get; set; }
}
