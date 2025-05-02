public enum TransactionType
{
    Income, 
    Expense  
}

namespace FinanceTracker.Server.Models.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string? BusinessName { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public string? Notes { get; set; }
        public string? ReceiptUrl { get; set; }
        public TransactionType Type { get; set; } = TransactionType.Expense;

        // Foreign keys
        public Guid? CategoryId { get; set; }
        public Guid? PaymentMethodId { get; set; }
        public Guid? UserId { get; set; }

        // Nav properties
        public Category Category { get; set; }
        public Reimbursement Reimbursement { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public User User { get; set; }

        // Credit card
        public bool IsCreditCardPurchase { get; set; } = false;
        public int? Installments { get; set; }
        public List<Installment> InstallmentsList { get; set; } = new List<Installment>();
    }
}
