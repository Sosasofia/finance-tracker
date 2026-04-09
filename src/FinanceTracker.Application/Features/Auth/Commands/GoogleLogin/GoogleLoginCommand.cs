using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Application.Features.Auth.Commands.GoogleLogin;

public class GoogleLoginCommand
{
    [Required]
    public required string IdToken { get; set; } 
}
