using FinanceTracker.Server.Models.DTOs;

namespace FinanceTracker.Server.Models.DTOs.Response
{
    public class TransactionResponse
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public string? Notes { get; set; }
        public string? ReceiptUrl { get; set; }
        public TransactionType Type { get; set; }
        public Guid CategoryId { get; set; }
        public Guid PaymentMethodId { get; set; }
        public Guid UserId { get; set; }
        public IEnumerable<InstallmentResponse> Installments { get; set; }
        public ReimbursementDTO Reimbursement { get; set; }
    }
}
