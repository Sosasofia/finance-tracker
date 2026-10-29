using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Application.Features.Auth.Models;

namespace FinanceTracker.Application.Features.Auth.Commands.GoogleLogin;

public record GoogleLoginCommand(string IdToken) : IRequest<AuthResponseDto>;
