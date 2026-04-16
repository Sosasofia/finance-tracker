using FinanceTracker.Application.Features.Auth.Models;
using MediatR;

namespace FinanceTracker.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    string Name,
    string Email,
    string Password
) : IRequest<AuthResponseDto>;
