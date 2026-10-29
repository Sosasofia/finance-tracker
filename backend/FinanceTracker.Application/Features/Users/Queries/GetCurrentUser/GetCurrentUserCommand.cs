using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Application.Features.Auth.Models;
using FinanceTracker.Application.Features.Users.Models;

namespace FinanceTracker.Application.Features.Users.Queries.GetCurrentUser;

public record GetCurrentUserQuery : IRequest<UserResponse>;
