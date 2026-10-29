using FinanceTracker.Application.Features.Users.Models;
using MediatR;

namespace FinanceTracker.Application.Features.Users.Commands.CreateUser;

public record CreateUserCommand(
    string Email,
    string Password,
    string? Name = null,
    string Provider = "local"
) : IRequest<UserResponse>;
