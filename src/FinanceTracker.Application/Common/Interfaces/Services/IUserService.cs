using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Common.Interfaces.Services;

public interface IUserService
{
    Task<bool> ExistsByAsync(Guid id);
    
    /// <summary>
    /// Processes a Google login by creating or retrieving a user account based on the provided Google account details.
    /// </summary>
    /// <remarks>This method ensures that a user account exists for the provided Google account details. If an account
    /// does not already exist, it will be created.</remarks>
    /// <param name="email">The email address associated with the Google account. This value cannot be null or empty.</param>
    /// <param name="name">The name of the user as provided by the Google account. This value cannot be null or empty.</param>
    /// <param name="pictureUrl">The URL of the user's profile picture from the Google account. This value can be null if no picture is available.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="User"/> object
    /// corresponding to the authenticated user.</returns>
    Task<User> ProcessGoogleLoginAsync(string email, string name, string pictureUrl);
}
