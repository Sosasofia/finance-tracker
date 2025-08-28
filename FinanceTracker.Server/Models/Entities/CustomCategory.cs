namespace FinanceTracker.Server.Models.Entities
{
    public class CustomCategory
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Color { get; set; }
        public string? Icon { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public Guid UserId { get; set; }
    }
}
