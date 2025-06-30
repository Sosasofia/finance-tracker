using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Server.Models.DTOs.Request
{
    public class AuthRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email format is invalid.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string Password { get; set; }

        public string? Role { get; set; } = "User";
    }
}
