namespace FinanceTracker.Server.Models.DTOs
{
    public class TransactionCreateDTO
    {
        public decimal Amount { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public string? Notes { get; set; }
        public string? ReceiptUrl { get; set; }
        public TransactionType Type { get; set; } = TransactionType.Expense;
        public Guid? CategoryId { get; set; }
        public Guid? PaymentMethodId { get; set; }

        // Credit card
        public bool IsCreditCardPurchase { get; set; } = false;
        public InstallmentDTO? Installment { get; set; }

        // Reimburstment
        public bool IsReimbursement { get; set; } = false;
        public ReimbursementDTO? Reimbursement { get; set; }
    }
}
