using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Server.Models.DTOs
{
    public class InstallmentDTO
    {
        [Required]
        [Range(1, 12, ErrorMessage = "Number of installments must be between 1 and 12.")]
        public int Number { get; set; }
        public int? Interest { get; set; } = null;
    }
}
