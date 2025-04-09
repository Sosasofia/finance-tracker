namespace FinanceTracker.Server.Models.DTOs
{
    public class TransactionCreateDTO
    {
        public decimal Amount { get; set; }
        public string? BussinessName { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public string? Notes { get; set; }
        public string? ReceiptUrl { get; set; }
        public TransactionType Type { get; set; } = TransactionType.Expense;
        public Guid CategoryId { get; set; }
        public Guid PaymentMethodId { get; set; }
        public Guid UserId { get; set; }

        // Credit card
        public bool IsCreditCardPurchase { get; set; } = false;
        public InstallmentDTO? Installment { get; set; }

        // Reimburstment
        public bool IsReimbursment { get; set; } = false;
        public ReimburstmentDTO? ReimburstmentDTO { get; set; }
    }
}
