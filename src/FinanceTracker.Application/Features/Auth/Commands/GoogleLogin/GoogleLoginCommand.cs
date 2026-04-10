using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Application.Features.Auth.Commands.GoogleLogin;

public record GoogleLoginCommand(
    [Required(ErrorMessage = "Google ID Token is required.")]
    string IdToken
);
