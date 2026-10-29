using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Application.Features.Auth.Models;

namespace FinanceTracker.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<AuthResponseDto>;
