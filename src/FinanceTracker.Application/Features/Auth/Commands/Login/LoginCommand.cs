using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email format is invalid.")]
    string Email,

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
    string Password
);
