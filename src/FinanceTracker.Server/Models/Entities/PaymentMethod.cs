namespace FinanceTracker.Server.Models.Entities
{
    public class PaymentMethod
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; } // credit card, debit card, cash, bank transfer, etc.
    }
}
