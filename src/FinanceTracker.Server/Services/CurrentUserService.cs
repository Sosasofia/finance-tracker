using System.Security.Claims;
using FinanceTracker.Application.Common.Interfaces.Security;

namespace FinanceTracker.Server.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid UserId()
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null && Guid.TryParse(claim.Value, out var userId))
            {
                return userId;
            }

            throw new UnauthorizedAccessException("Missing or invalid user ID claim");
        }
    }
}
