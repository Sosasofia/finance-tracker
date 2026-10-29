namespace FinanceTracker.Application.Features.Transactions.Models;

public record InstallmentDto(
    int Number,
    int? Interest = null
);
