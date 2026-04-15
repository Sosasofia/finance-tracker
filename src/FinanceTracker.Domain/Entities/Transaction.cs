using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Exceptions;
using FinanceTracker.Domain.ValueObjects;

namespace FinanceTracker.Domain.Entities;

public class Transaction
{
    private readonly List<Installment> _installments = [];

    public Guid Id { get; private set; }
    public Money Money { get; private set; }
    public string Name { get; private set; }
    public DateTime Date { get; private set; }
    public TransactionType Type { get; private set; }
    public string? Description { get; private set; }
    public string? Notes { get; private set; }
    public string? ReceiptUrl { get; private set; }

    // Audit properties
    public DateTime CreatedAt { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    public Guid? CategoryId { get; private set; }
    public Guid? PaymentMethodId { get; private set; }
    public Guid? ReimbursementId { get; private set; }

    public virtual Category Category { get; private set; }
    public virtual Reimbursement Reimbursement { get; private set; }
    public virtual PaymentMethod PaymentMethod { get; private set; }
    public virtual User User { get; private set; }

    public bool IsCreditCardPurchase { get; private set; }

    public IReadOnlyCollection<Installment> Installments => _installments.AsReadOnly();

    private Transaction() { }

    public static Transaction Create(
        Money money,
        string name,
        DateTime date,
        TransactionType type,
        Guid userId,
        Guid categoryId,
        Guid paymentMethodId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name cannot be empty.");

        if (userId == Guid.Empty)
            throw new DomainException("Transaction must belong to a user.");


        return new Transaction
        {
            Id = Guid.NewGuid(),
            Money = money,
            Name = name,
            Date = date,
            Type = type,
            UserId = userId,
            CategoryId = categoryId,
            PaymentMethodId = paymentMethodId,
            CreatedAt = DateTime.UtcNow
        };
    }


    public void UpdateDetails(string newName, string? newDescription)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new DomainException("The new name is invalid.");

        Name = newName;
        Description = newDescription;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void GenerateInstallments(int numberOfInstallments)
    {
        if (numberOfInstallments <= 0)
            throw new DomainException("The number of installments must be greater than zero.");

        _installments.Clear();
        IsCreditCardPurchase = true;

        var installmentAmount = Math.Round(Money.Amount / numberOfInstallments, 2);

        var installmentMoney = Money.Create(installmentAmount, Money.Currency);


        for (int i = 1; i <= numberOfInstallments; i++)
        {
            var dueDate = Date.AddMonths(i);
            var installment = Installment.Create(installmentMoney, i, dueDate);
            _installments.Add(installment);
        }
    }

    public void AddReimbursement(Money amount, string reason)
    {
        if (amount.Currency != Money.Currency)
            throw new DomainException($"Cannot reimburse in {amount.Currency} for a transaction made in {Money.Currency}.");

        if (amount.Amount > Money.Amount)
            throw new DomainException("Reimbursement cannot exceed the original transaction amount.");

        Reimbursement = Reimbursement.Create(amount, reason, Id);
        ReimbursementId = Reimbursement.Id;
    }

    public void ChangeAmount(Money newMoney)
    {
        Money = newMoney;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void ChangeDate(DateTime newDate)
    {
        if (newDate.Date > DateTime.UtcNow.Date)
        {
            throw new DomainException("Transaction date cannot be in the future.");
        }

        Date = newDate.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(newDate, DateTimeKind.Utc)
            : newDate.ToUniversalTime();

        LastModifiedAt = DateTime.UtcNow;
    }

    public void AttachReceipt(string receiptUrl)
    {
        if (string.IsNullOrWhiteSpace(receiptUrl))
            throw new DomainException("Receipt URL cannot be empty.");

        ReceiptUrl = receiptUrl;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void AssignCategory(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
            throw new DomainException("Category ID cannot be empty.");

        CategoryId = categoryId;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void RemoveCategory()
    {
  
        CategoryId = null;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void AssignPaymentMethod(Guid paymentMethodId)
    {
        if (paymentMethodId == Guid.Empty)
            throw new DomainException("Payment Method ID cannot be empty.");

        PaymentMethodId = paymentMethodId;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void RemovePaymentMethod()
    {

        PaymentMethodId = null;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
    }
}
