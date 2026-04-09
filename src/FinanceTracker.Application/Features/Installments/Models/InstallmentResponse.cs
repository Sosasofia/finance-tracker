using FinanceTracker.Domain.ValueObjects;

namespace FinanceTracker.Application.Features.Installments.Models;

public class InstallmentResponse
{
    public Guid Id { get; set; }
    public int InstallmentNumber { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "ARS";
    public DateTime DueDate { get; set; }
    public DateTime? PaymentDate { get; set; }
    public bool IsPaid { get; set; }
}
