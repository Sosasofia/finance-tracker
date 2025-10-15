using FinanceTracker.Application.Features.Auth;

namespace FinanceTracker.Application.Common.Interfaces.Services;

public interface IAuthApplicationService
{
    /// <summary>
    /// Registers a new user with the specified email and password.
    /// </summary>
    /// <remarks>Ensure that the provided email is unique and the password meets the application's security
    /// requirements.  This method performs validation and may fail if the email is already registered or the password
    /// is invalid.</remarks>
    /// <param name="email">The email address of the user to register. This cannot be null or empty and must be a valid email format.</param>
    /// <param name="password">The password for the user to register. This cannot be null or empty and should meet the required password
    /// complexity rules.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AuthResponse"/> object
    /// if the registration is successful; otherwise, <see langword="null"/> if the registration fails.</returns>
    Task<AuthResponse> RegisterUserAsync(string email, string password);
    /// <summary>
    /// Authenticates a user with the provided credentials and returns an authentication response.
    /// </summary>
    /// <remarks>This method performs user authentication and may involve external systems or services. Ensure
    /// that the provided credentials are valid and properly encoded if necessary.</remarks>
    /// <param name="username">The username of the user attempting to log in. Cannot be null or empty.</param>
    /// <param name="password">The password associated with the specified username. Cannot be null or empty.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AuthResponse"/> object
    /// if the login is successful; otherwise, <see langword="null"/>.</returns>
    Task<AuthResponse> LoginUserAsync(string username, string password);
    /// <summary>
    /// Authenticates a user with Google using the provided ID token.
    /// </summary>
    /// <remarks>Ensure that the ID token is obtained from a trusted source, such as Google's  authentication
    /// services, and has not expired before calling this method.</remarks>
    /// <param name="idToken">The ID token issued by Google, representing the user's identity.  This token must be a valid, non-null string.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains  a string representing the
    /// authentication token for the user, which can be  used for subsequent API calls.</returns>
    Task<AuthResponse> AuthenticateWithGoogleAsync(string idToken);
}
