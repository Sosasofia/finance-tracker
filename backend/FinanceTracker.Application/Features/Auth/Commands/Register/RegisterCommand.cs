using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Application.Features.Auth.Models;

namespace FinanceTracker.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    string Name,
    string Email,
    string Password
) : IRequest<AuthResponseDto>;
