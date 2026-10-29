namespace FinanceTracker.Application.Features.Transactions.Models;

public record ReimbursementDto(
    decimal Amount,
    DateTime Date,
    string? Reason = null
);
