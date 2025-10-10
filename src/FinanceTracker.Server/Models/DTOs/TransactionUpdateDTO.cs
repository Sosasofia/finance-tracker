using FinanceTracker.Server.Enums;
using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Server.Models.DTOs
{
    public class TransactionUpdateDTO
    {
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
        public string Name { get; set; }
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }
        public DateTime Date { get; set; }
        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
        public string Notes { get; set; }
        public string ReceiptUrl { get; set; }
        [EnumDataType(typeof(TransactionType), ErrorMessage = "Invalid transaction type.")]
        public TransactionType Type { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? PaymentMethodId { get; set; }

        public List<string> Validate()
        {
            var errors = new List<string>();

            if (Date > DateTime.UtcNow)
            {
                errors.Add("Transaction date cannot be in the future.");
            }

            return errors;
        }
    }
}

