using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Server.Models.DTOs.Request
{
    [Keyless]
    public class AuthRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string? Role { get; set; } = "User";
    }
}
