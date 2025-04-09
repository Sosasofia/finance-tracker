namespace FinanceTracker.Server.Models.DTOs
{
    public class ReimburstmentDTO
    {
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string? Reason { get; set; }
    }
}
