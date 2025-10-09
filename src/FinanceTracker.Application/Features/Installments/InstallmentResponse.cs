namespace FinanceTracker.Application.Features.Installments;

public class InstallmentResponse
{
    public Guid Id { get; set; }
    public int InstallmentNumber { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? PaymentDate { get; set; }
    public bool IsPaid { get; set; }
}