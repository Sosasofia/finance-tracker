namespace FinanceTracker.Server.Models.DTOs
{
    public class ReimbursementDTO
    {
        public Guid? Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string? Reason { get; set; }
    }
}
