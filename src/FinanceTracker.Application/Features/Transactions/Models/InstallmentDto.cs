using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Application.Features.Transactions.Models;

public record InstallmentDto(
    [Required]
    [Range(1, 12, ErrorMessage = "Number of installments must be between 1 and 12.")]
    int Number,
    int? Interest = null
);
