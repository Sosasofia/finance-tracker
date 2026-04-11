namespace FinanceTracker.Application.Features.Transactions.Models;

public record InstallmentResponse(
    Guid Id,
    int InstallmentNumber,
    decimal Amount,
    DateTime DueDate,
    bool IsPaid,
    string Currency = "ARS",
    DateTime? PaymentDate = null
);
