using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Server.Models.DTOs
{
    public class ReimbursementDTO
    {
        [Required]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Reimbursement amount must be greater than zero.")]
        public decimal Amount { get; set; }
        [Required]

        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string? Reason { get; set; }

        public List<string> Validate()
        {
            var errors = new List<string>();

            if (Date > DateTime.Now)
            {
                errors.Add("Transaction date cannot be in the future.");
            }

            return errors;
        }
    }
}
