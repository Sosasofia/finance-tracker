namespace FinanceTracker.Domain.Entities;
public class Installment
{
    public Guid Id { get; set; }
    public int InstallmentNumber { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? PaymentDate { get; set; }
    public bool IsPaid { get; set; } = false;

    // Foreign keys
    public Guid TransactionId { get; set; }

    // Nav properties
    public Transaction Transaction { get; set; }
}
