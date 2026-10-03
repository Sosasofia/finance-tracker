using FinanceTracker.Application.Features.Auth.Models;
using MediatR;

namespace FinanceTracker.Application.Features.Users.Queries.GetCurrentUser;

public record GetCurrentUserQuery : IRequest<UserSessionDto>;
