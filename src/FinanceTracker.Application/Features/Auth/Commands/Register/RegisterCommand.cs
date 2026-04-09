using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Application.Features.Auth.Commands.Register;

public class RegisterCommand
{
    [Required(ErrorMessage = "Name is required.")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email format is invalid.")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
    public required string Password { get; set; }

    public string Role { get; set; } = "User";

    public string Provider { get; set; } = "Local";
}
