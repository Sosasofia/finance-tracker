using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    [Required(ErrorMessage = "Name is required.")]
    string Name,

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email format is invalid.")]
    string Email,

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
    string Password
);
