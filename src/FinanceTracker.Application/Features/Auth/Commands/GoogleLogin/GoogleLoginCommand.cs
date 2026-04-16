using FinanceTracker.Application.Features.Auth.Models;
using MediatR;

namespace FinanceTracker.Application.Features.Auth.Commands.GoogleLogin;

public record GoogleLoginCommand(string IdToken) : IRequest<AuthResponseDto>;
