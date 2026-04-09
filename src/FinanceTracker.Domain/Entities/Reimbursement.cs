using FinanceTracker.Domain.ValueObjects;

namespace FinanceTracker.Domain.Entities;

public class Reimbursement
{
    public Guid Id { get; private set; }
    public Money Money { get; private set; }
    public DateTime Date { get; private set; }
    public string? Reason { get; private set; }

    public Guid TransactionId { get; private set; }

    public virtual Transaction Transaction { get; private set; }
    private Reimbursement() { }

    public static Reimbursement Create(Money money, string? reason, Guid transactionId)
    {
        return new Reimbursement
        {
            Id = Guid.NewGuid(),
            Money = money,
            Reason = reason,
            Date = DateTime.UtcNow,
            TransactionId = transactionId
        };
    }
}
