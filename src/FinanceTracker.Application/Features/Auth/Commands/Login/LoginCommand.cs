using FinanceTracker.Application.Features.Auth.Models;
using MediatR;

namespace FinanceTracker.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<AuthResponseDto>;
