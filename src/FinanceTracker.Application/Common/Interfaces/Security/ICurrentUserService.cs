namespace FinanceTracker.Application.Common.Interfaces.Security;

public interface ICurrentUserService
{
    /// <summary>
    /// Retrieves the user ID from the current HTTP context, if available.
    /// </summary>
    /// <remarks>This method extracts the user ID from the <see cref="ClaimTypes.NameIdentifier"/>
    /// claim in the current user's identity. If the claim is not present or cannot be parsed as a <see
    /// cref="Guid"/>, the method returns <see langword="null"/>.</remarks>
    /// <returns>A <see cref="Guid"/> representing the user ID if the claim is present and valid; otherwise, <see
    /// langword="null"/>.</returns>
    Guid? UserId();
}
