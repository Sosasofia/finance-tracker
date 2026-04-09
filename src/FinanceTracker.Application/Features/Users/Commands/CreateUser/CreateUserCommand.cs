using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommand
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public required string Email { get; set; }

    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
    public required string Password { get; set; }

    public string Provider { get; set; } = "local";
}
