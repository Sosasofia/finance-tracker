namespace FinanceTracker.Application.Common.Interfaces.Security;

public interface ICurrentUserService
{
    /// <summary>
    /// Retrieves the unique identifier for the user.
    /// </summary>
    /// <returns>A <see cref="Guid"/> representing the user's unique identifier.</returns>
    Guid UserId();
}
