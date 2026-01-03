using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Application.Features.Auth;

public class AuthRequestDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email format is invalid.")]
    required public string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
    required public string Password { get; set; }
}
