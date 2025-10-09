namespace FinanceTracker.Domain.Entities;

public class Reimbursement
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string? Reason { get; set; }

    // Foreign keys
    public Guid TransactionId { get; set; }

    // Nav properties
    public Transaction Transaction { get; set; }
}
