using FinanceTracker.Application.Features.Auth;

namespace FinanceTracker.Application.Interfaces.Services;

public interface IAuthApplicationService
{
    Task<AuthResponse?> RegisterUserAsync(string email, string password);
    Task<AuthResponse?> LoginUserAsync(string username, string password);
    /// <summary>
    /// Authenticates a user with Google using the provided ID token.
    /// </summary>
    /// <remarks>Ensure that the ID token is obtained from a trusted source, such as Google's  authentication
    /// services, and has not expired before calling this method.</remarks>
    /// <param name="idToken">The ID token issued by Google, representing the user's identity.  This token must be a valid, non-null string.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains  a string representing the
    /// authentication token for the user, which can be  used for subsequent API calls.</returns>
    Task<string> AuthenticateWithGoogleAsync(string idToken);
}
