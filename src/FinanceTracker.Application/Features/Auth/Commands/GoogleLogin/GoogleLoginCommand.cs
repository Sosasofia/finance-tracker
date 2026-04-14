using System.ComponentModel.DataAnnotations;
using FinanceTracker.Application.Features.Auth.Models;
using MediatR;

namespace FinanceTracker.Application.Features.Auth.Commands.GoogleLogin;

public record GoogleLoginCommand(
    [Required(ErrorMessage = "Google ID Token is required.")]
    string IdToken
) : IRequest<AuthResponseDto>;
