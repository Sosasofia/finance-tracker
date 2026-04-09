using FinanceTracker.Domain.ValueObjects;

namespace FinanceTracker.Domain.Entities;

public class Installment
{
    public Guid Id { get; private set; }
    public int InstallmentNumber { get; private set; }
    public Money Money { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime? PaymentDate { get; private set; }
    public bool IsPaid { get; private set; } 
    public Guid TransactionId { get; private set; }
    public virtual Transaction Transaction { get; private set; }

    private Installment() { }
    
    public static Installment Create(Money money, int number, DateTime dueDate)
    {
        return new Installment
        {
            Id = Guid.NewGuid(),
            Money = money,
            InstallmentNumber = number,
            DueDate = dueDate,
            IsPaid = false
        };
    }

    public void MarkAsPaid()
    {
        IsPaid = true;
        PaymentDate = DateTime.UtcNow;
    }
}
