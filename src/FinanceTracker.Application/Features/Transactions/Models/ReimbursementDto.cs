using System.ComponentModel.DataAnnotations;
using FinanceTracker.Application.Common.Validators;

namespace FinanceTracker.Application.Features.Transactions.Models;

public record ReimbursementDto(
    [Required]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Reimbursement amount must be greater than zero.")]
    decimal Amount,

    [Required]
    [DataCannotBeInTheFuture]
    DateTime Date,

    string? Reason = null
);
