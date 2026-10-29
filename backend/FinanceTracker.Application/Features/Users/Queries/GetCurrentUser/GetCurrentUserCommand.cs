using FinanceTracker.Application.Features.Auth.Models;
using FinanceTracker.Application.Features.Users.Models;
using MediatR;

namespace FinanceTracker.Application.Features.Users.Queries.GetCurrentUser;

public record GetCurrentUserQuery : IRequest<UserResponse>;
